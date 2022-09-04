using CodingChallenge.Application.TVMaze.Base;
using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Application.TVMaze.Commands.Scrape;
using CodingChallenge.Application.TVMaze.Queries;
using CodingChallenge.Application.TVMaze.Queries.Token;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.EventQueueProcessor;

public class TVMazeScrapeCommandController
{
    private readonly ISender _mediator;

    private readonly ILogger _logger;

    public TVMazeScrapeCommandController(ILogger logger, ISender sender)
    {
        _logger = logger;
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
            _logger.LogDebug($"command with token id is ScrapeCommand");
            return await ScrapeAsync((command as ScrapeCommand)!);
        }
        return null;
    }

    public async Task<ScrapeCommandResponse> ScrapeAsync(ScrapeCommand ScrapeCommand)
    {
        _logger.LogDebug($"scrape  command is called for token id {ScrapeCommand.index}");
        return await _mediator.Send<ScrapeCommandResponse>(ScrapeCommand);
    }
    public async Task<AddScrapeTaskCommandResponse> AddScrapeTaskAsync(AddScrapeTaskCommand addScrapeTaskCommand)
    {
        return await _mediator.Send<AddScrapeTaskCommandResponse>(addScrapeTaskCommand);
    }
    public async Task<TVMazeRecordDto> GetTokenByIdAsync(GetTVMazeItemByIndexQuery query)
    {
        _logger.LogDebug($"{query.Index} - token query is called for token id ");
        return await _mediator.Send<TVMazeRecordDto>(query);
    }
}
