using Microsoft.Extensions.Logging;
using CodingChallenge.Infrastructure.Extensions;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace CodingChallenge.Infrastructure.Repository;

public interface IApplicationDynamoDBBase<T> where T : class
{
    Task DeleteAsync(List<T> dataModelItems);
    Task<T> GetAsync(string id);
    Task SaveAsync(List<T> dataModelItems);
    Task UpdateAsync(List<T> dataModelItems);
}

public class ApplicationDynamoDBBase<T> : DynamoDBRepository, IApplicationDynamoDBBase<T> where T : class
{
    protected virtual DynamoDBOperationConfig DynamoDBOperationConfig { get; }

    public ApplicationDynamoDBBase(ILogger logger, AWSAppProject awsApplication) : base(logger, awsApplication)
    {
        DynamoDBOperationConfig = GetOperationConfig(AwsApplication.GetDynamodbTableName(typeof(T)));
    }

    public async Task DeleteAsync(List<T> dataModelItems)
    {
        var dataModelBatch = Context.CreateBatchWrite<T>(DynamoDBOperationConfig);
        dataModelBatch.AddDeleteItems(dataModelItems);
        await dataModelBatch.ExecuteAsync();
    }

    public async Task<T> GetAsync(string id)
    {
        var result = await Context.LoadAsync<T>(id, DynamoDBOperationConfig);
        return result;
    }
    public async Task<T> GetAsync(int id)
    {
        var result = await Context.LoadAsync<T>(id, DynamoDBOperationConfig);
        return result;
    }

    public async Task<Tuple<List<T>, string>> GetListAsync(int limit, string? paginationToken = null)
    {

        var table = Context.GetTargetTable<T>(DynamoDBOperationConfig);

        var config = new ScanOperationConfig();
        config.Limit = limit;
        //config. Filter = new QueryFilter(sortKeyField, QueryOperator.Equal, 2012);
        if (!string.IsNullOrWhiteSpace(paginationToken))
        {
            config.PaginationToken = paginationToken;
        }
        var results = table.Scan(config);
        var items = await results.GetNextSetAsync();
        IEnumerable<T> employees = Context.FromDocuments<T>(items);
        var returnList = employees.ToList();
        return new Tuple<List<T>, string>(returnList, results.PaginationToken);
    }

    public async Task<List<T>> GetBySortKeyAsync(string sortKeyField, string sortKeyValue)
    {
        return await Context.ScanAsync<T>(
            new ScanCondition[] { new ScanCondition(sortKeyField, ScanOperator.Equal, sortKeyValue) }, DynamoDBOperationConfig)
            .GetRemainingAsync();
    }

    public async Task SaveAsync(List<T> dataModelItems)
    {
        var dataModelBatch = Context.CreateBatchWrite<T>(DynamoDBOperationConfig);
        dataModelBatch.AddPutItems(dataModelItems);
        await dataModelBatch.ExecuteAsync();
    }

    public async Task UpdateAsync(List<T> dataModelItems)
    {
        var dataModelBatch = Context.CreateBatchWrite<T>(DynamoDBOperationConfig);
        dataModelBatch.AddPutItems(dataModelItems);
        await dataModelBatch.ExecuteAsync();
    }
}
