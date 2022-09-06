using CodingChallenge.Application.TVMaze.Base;
using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Application.TVMaze.Commands.Scrape;
using CodingChallenge.Application.TVMaze.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.GetItemsLambda;

public class TVMazeScrapeCommandController
{
    private readonly ISender _mediator;

    private readonly ILogger _logger;

    public TVMazeScrapeCommandController(ILogger logger, ISender sender)
    {
        _logger = logger;
        _mediator = sender;
    }

    public async Task<PagedList<TVMazeRecordDto>> GetList(GetTVMazeItemsQuery query)
    {
        return await _mediator.Send<PagedList<TVMazeRecordDto>>(query);
    }
}
