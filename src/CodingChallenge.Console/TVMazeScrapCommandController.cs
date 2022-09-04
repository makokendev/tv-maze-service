using System.Threading.Tasks;
using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Application.TVMaze.Commands.Scrape;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Console;
public class TVMazeScrapeCommandController
{
    private readonly ISender _mediator;

    private readonly ILogger _logger;

    public TVMazeScrapeCommandController(ILogger logger, ISender sender)
    {
        _logger = logger;
        _mediator = sender;
    }

    public async Task<AddScrapeTaskCommandResponse> AddScrapeTaskAsync(AddScrapeTaskCommand addScrapeTaskCommand)
    {
        _logger.LogDebug($"Add scrape task command is called for token id");
        return await _mediator.Send<AddScrapeTaskCommandResponse>(addScrapeTaskCommand);
    }
}
