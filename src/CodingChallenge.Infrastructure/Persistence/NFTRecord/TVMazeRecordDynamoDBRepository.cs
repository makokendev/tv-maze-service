using Microsoft.Extensions.Logging;
using AutoMapper;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Infrastructure.Repository;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using CodingChallenge.Infrastructure.Extensions;
using Newtonsoft.Json;
using RestSharp;
using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Domain.Entities;
using System.Net;
using CodingChallenge.Domain.Entities.TvMaze;

namespace CodingChallenge.Infrastructure.Persistence.TVMazeRecord;

public class TVMazeRecordDynamoDBRepository : ApplicationDynamoDBBase<TVMazeRecordDataModel>, ITVMazeRecordRepository
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly AmazonSimpleNotificationServiceClient _snsClient;

    //TODO - get from CDK project!
    public const string eventTopicSuffix = "eventtopic";
    private const string ProductionType = "Movie";
    private const string baseurl = "https://api.tvmaze.com";
    private readonly string TopicArn;

    public TVMazeRecordDynamoDBRepository(IMapper mapper, ILogger logger, AWSAppProject awsApplication) : base(logger, awsApplication)
    {
        _mapper = mapper;
        _logger = logger;
        _snsClient = new AmazonSimpleNotificationServiceClient();
        _logger.LogInformation($"AWS OBJECT -> {JsonConvert.SerializeObject(awsApplication)}");
        TopicArn = $"arn:aws:sns:us-east-1:476631482508:{awsApplication.GetResourceName(eventTopicSuffix)}";
    }

    public async Task<TVMazeCastDataResponse> GetTVMazeCastById(int id)
    {
        var retObj = new TVMazeCastDataResponse();
        var response = await TVMazeCastByShowIdHttpGetCall(id);

        if (response == null)
        {
            retObj.IsSuccessful = false;
            _logger.LogInformation($"{id} - response--EXITING. response is null");
            return retObj;
        }

        _logger.LogInformation($"{id} - StatusCode response code is {response.StatusCode}");

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            _logger.LogInformation($"{id} -  error mesage code is {response.ErrorMessage}");
        }

        _logger.LogInformation($"{id} - response json is {JsonConvert.SerializeObject(response)}");

        if (response.IsSuccessful)
        {
            _logger.LogInformation($"{id} - response--SUCCESS.  getting tv maze cast by id :{id} - SUCCESS");
            retObj.IsSuccessful = true;
            retObj.RateLimited = false;
            retObj.CastList = JsonConvert.DeserializeObject<List<TVMazeCastItem>>(response.Content!)!;
            return retObj;
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            retObj.IsSuccessful = false;
            retObj.RateLimited = true;
            _logger.LogInformation($"{id} - response--TOOMANY.getting tv maze cast by id :{id} - TOO MANY");
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            retObj.IsSuccessful = false;
            retObj.NotFound = true;
            _logger.LogInformation($"{id} - response--NOTFOUND.");
        }
        else
        {
            retObj.IsSuccessful = false;
            retObj.RateLimited = false;
            _logger.LogInformation($"{id} - response--ERROR.  getting tv maze cast by id :{id} - ERROR");
        }

        return retObj;
    }

    private async Task<RestResponse?> TVMazeCastByShowIdHttpGetCall(int id)
    {
        try
        {
            _logger.LogInformation($"{id} - getting tv maze cast by id");

            var options = new RestClientOptions(baseurl)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);

            var request = new RestRequest($"shows/{id}/cast");

            return await client.GetAsync(request);
        }
        catch (WebException ex)
        {
            _logger.LogError($"{id} - web exception error {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"{id} - HttpRequestException -  {ex.Message} - data: {JsonConvert.SerializeObject(ex.Data)}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{id} - generic exception error {ex.Message}. Type {ex.GetType().Name}");
        }

        return null;
    }

    public async Task<TVMazeCastDataResponse> ScrapeAsync(int index)
    {
        var item = await this.GetAsync(index);
        if (item?.CastList.Any() == true)
        {
            _logger.LogWarning($"{index} already exists! Dynamodb repo");
            var entity = new TVMazeCastDataResponse()
            {
                IsSuccessful = true,
                AlreadyStored = true,
                CastList = item.CastList
            };
            return entity;
        }
        var result = await GetTVMazeCastById(index);
        if (result.IsSuccessful)
        {
            _logger.LogInformation($"{index} - get tv maze cast by id is successfull id is ");
            if (result.CastList == null || !result.CastList.Any())
            {
                _logger.LogInformation($"{index} - cast list is empty");
                return result;
            }
            var SortedList = result.CastList.OrderBy(o=>o.person.birthday).ToList();
            var entity = new TVMazeRecordEntity()
            {
                Index = index,
                CastList = SortedList,
                ProductionType = ProductionType
            };

            var dataModel = _mapper.Map<TVMazeRecordEntity, TVMazeRecordDataModel>(entity);
            _logger.LogInformation($"dataModel index is -> {dataModel.TVMazeIndex}");
            dataModel.Created = DateTime.UtcNow;
            await this.SaveAsync(new List<TVMazeRecordDataModel>(){
            dataModel
        });

        }
        else if (!result.IsSuccessful && result.RateLimited)
        {
            _logger.LogDebug($"{index} - get tv maze cast by id is rate limited");
        }
        else
        {
            _logger.LogDebug($"{index} - things have gone wrong dude");
        }
        return result;
    }


    public async Task AddScrapeTaskAsync(AddScrapeTaskCommand command)
    {
        var publishRequest = new PublishRequest()
        {
            Message = JsonConvert.SerializeObject(command),
            TopicArn = TopicArn
        };

        await _snsClient.PublishAsync(publishRequest);
    }

    public async Task<TVMazeRecordEntity> GetByIndexAsync(string index)
    {
        _logger.LogDebug($"{index} -Get By Token repo action is being executed... Token Id is {index}");
        var result = await GetAsync(index);
        var mappedEntity = _mapper.Map<TVMazeRecordDataModel, TVMazeRecordEntity>(result);
        _logger.LogDebug($"{index} - Get By Token repo action is successfully executed for token with Id {index}. Returning result");
        return mappedEntity;
    }

    public async Task<IEnumerable<TVMazeRecordEntity>> GetItemListAsync(int pageSize,string? paginationToken=null)
    {
        _logger.LogInformation($"page size is {pageSize}");
        _logger.LogInformation($"paginationToken is {paginationToken}");
        var result = await GetListAsync(pageSize,paginationToken);
        var mappedEntity = _mapper.Map<List<TVMazeRecordDataModel>, IEnumerable<TVMazeRecordEntity>>(result.Item1);
        return mappedEntity;
    }

}
