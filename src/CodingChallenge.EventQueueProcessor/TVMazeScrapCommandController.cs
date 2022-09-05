using CodingChallenge.Application.TVMaze.Base;
using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Application.TVMaze.Commands.Scrape;
using CodingChallenge.Application.TVMaze.Queries;
using MediatR;

namespace CodingChallenge.EventQueueProcessor;

public class TVMazeScrapeCommandController
{
    private readonly ISender _mediator;

    public TVMazeScrapeCommandController(ISender sender)
    {
        _mediator = sender;
    }

    public async Task<List<TVMazeScrapeCommandResponseBase>?> ProcessCommandListAsync(List<TVMazeScrapeCommandBase> commandList)
    {
        var responseList = new List<TVMazeScrapeCommandResponseBase>();
        foreach (var command in commandList)
        {
            var result = await ExecuteTransactionCommandBaseAsync(command);
            if (result != null)
            {
                responseList.Add(result);
            }
        }
        return responseList;
    }

    public async Task<TVMazeScrapeCommandResponseBase?> ExecuteTransactionCommandBaseAsync(TVMazeScrapeCommandBase command)
    {
        if (command is ScrapeCommand)
        {
            return await ScrapeAsync((command as ScrapeCommand)!);
        }

        return null;
    }

    public async Task<ScrapeCommandResponse> ScrapeAsync(ScrapeCommand ScrapeCommand)
    {
        return await _mediator.Send(ScrapeCommand);
    }

    public async Task<AddScrapeTaskCommandResponse> AddScrapeTaskAsync(AddScrapeTaskCommand addScrapeTaskCommand)
    {
        return await _mediator.Send(addScrapeTaskCommand);
    }

    public async Task<TVMazeRecordDto> GetItemByIdAsync(GetTVMazeItemByIndexQuery query)
    {
        return await _mediator.Send(query);
    }
}
