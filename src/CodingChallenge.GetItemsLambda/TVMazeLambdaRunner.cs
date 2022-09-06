using CodingChallenge.Application.TVMaze.Queries;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.GetItemsLambda;

public class TVMazeLambdaRunner
{
    ILogger _logger;
    TVMazeScrapeCommandController _TVMazeRecordCommandHandler;
    public TVMazeLambdaRunner(ILogger logger, TVMazeScrapeCommandController handler)
    {
        _logger = logger;
        _TVMazeRecordCommandHandler = handler;
    }
    public async Task<PagedList<TVMazeRecordDto>> GetList(GetTVMazeItemsQuery query)
    {
        return await _TVMazeRecordCommandHandler.GetList(query);
    }
    

}