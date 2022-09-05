using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Application.TVMaze.Commands.Scrape;
using CodingChallenge.Application.TVMaze.Queries;

namespace CodingChallenge.EventQueueProcessor;

public class TVMazeLambdaRunner
{
    TVMazeScrapeCommandController _TVMazeRecordCommandHandler;

    public TVMazeLambdaRunner(TVMazeScrapeCommandController handler)
    {
        _TVMazeRecordCommandHandler = handler;
    }

    public async Task<ScrapeCommandResponse> SendScrapeCommand(int index)
    {
        return await _TVMazeRecordCommandHandler.ScrapeAsync(new ScrapeCommand(index));

    }

    public async Task<AddScrapeTaskCommandResponse> AddScrapeTaskAsync(AddScrapeTaskCommand addScrapeTaskCommand)
    {
        return await _TVMazeRecordCommandHandler.AddScrapeTaskAsync(addScrapeTaskCommand);
    }

    public async Task<TVMazeRecordDto> GetItemByIdAsync(GetTVMazeItemByIndexQuery query)
    {
        return await _TVMazeRecordCommandHandler.GetItemByIdAsync(query);
    }    
}