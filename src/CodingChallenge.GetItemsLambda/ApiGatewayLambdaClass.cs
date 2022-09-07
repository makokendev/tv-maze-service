using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CodingChallenge.Application;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace CodingChallenge.GetItemsLambda;

public class ApiGatewayLambdaClass
{
    public ILogger logger;
    public IConfiguration configuration;
    public IServiceProvider serviceProvider;
    public AWSAppProject awsApplication;

    public ApiGatewayLambdaClass()
    {
        configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables().Build();

        awsApplication = new AWSAppProject();
        configuration.GetSection(Constants.APPLICATION_ENVIRONMENT_VAR_PREFIX).Bind(awsApplication);

        logger = SetupLogger();

        var services = new ServiceCollection();
        ConfigureServices(services);
        serviceProvider = services.BuildServiceProvider();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationBaseDependencies();
        services.AddInfrastructureDependencies(configuration, logger);
        services.AddSingleton(logger);
        services.AddSingleton(awsApplication);

        services.AddTransient<TVMazeScrapeCommandController, TVMazeScrapeCommandController>();
        services.AddTransient<TVMazeLambdaRunner, TVMazeLambdaRunner>();
    }

    public ILogger SetupLogger()
    {
        return new CustomLambdaLoggerProvider(new CustomLambdaLoggerConfig()
        {
            LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            InfrastructureProject = awsApplication

        }).CreateLogger(nameof(ApiGatewayLambdaClass));
    }

    public async Task<APIGatewayProxyResponse> GetListHandlerAsync(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
    {
        try
        {
            var runner = serviceProvider.GetService<TVMazeLambdaRunner>();

            if (runner == null)
            {
                logger.LogInformation("runner is null");
                throw new NullReferenceException("runner is null... TVMazeLambdaRunner is not mapped");
            }

            // Extract paging parameters from querystring
            var paging = new PagingParameters(apigProxyEvent.QueryStringParameters ?? new Dictionary<string, string>());

            var paginatedResponse = await runner.GetList(
                new Application.TVMaze.Queries.GetTVMazeItemsQuery(paging.PageSize, paging.PaginationToken));

            return SuccessResponse(paginatedResponse);
        }
        catch (Exception ex)
        {
            return HandleGenericError<ErrorResponse>(ex);
        }
    }    

    public APIGatewayProxyResponse InternalErrorResponse<T>(T responseBodyModel)
    {
        return new APIGatewayProxyResponse
        {
            Body = JsonConvert.SerializeObject(responseBodyModel),
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
    public APIGatewayProxyResponse HandleGenericError<T>(Exception ex)
           where T : IResponse, new()
    {
        logger.LogError(ex, $"Exception occured in {nameof(T)}");

        return InternalErrorResponse(new T()
        {
            IsSuccess = false,
            ErrorResponseInfo = new ErrorResponseBase
            {
                ErrorCode = 0,
                ErrorMessage = $"Version:{awsApplication.Version} -- Message: {ex.Message}. Stack Trace: {ex.StackTrace}"
            }
        });
    }

    public APIGatewayProxyResponse SuccessResponse<T>(T responseBodyModel)
    {
        return new APIGatewayProxyResponse
        {
            Body = JsonConvert.SerializeObject(responseBodyModel),
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }, // This needs to be there to make CORS work!
                    { "Access-Control-Allow-Headers", "*" }
                }
        };
    }

}

internal class PagingParameters {
    const string pTokenKey = "pagetoken";
    const string sizeKey = "size";
    const int DefaultPageSize = 10;

    public string? PaginationToken { get; set;}
    public int PageSize { get; set;}

    public PagingParameters(IDictionary<string, string> parameters)
    {
        string? sSize = null;
        string? pToken = null;

        if (parameters != null)
        {
            parameters.TryGetValue(pTokenKey, out pToken);
            parameters.TryGetValue(sizeKey, out sSize);
        }

        if (!int.TryParse(sSize, out int size))
        {
            size = DefaultPageSize;
        }

        size = size <= 0 ? DefaultPageSize : size;

        PaginationToken = string.IsNullOrWhiteSpace(pToken) ? null : pToken; ;
        PageSize = size;
    }
}
