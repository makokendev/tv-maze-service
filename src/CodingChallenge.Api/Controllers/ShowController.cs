using CodingChallenge.Api.Mappers;
using CodingChallenge.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using CodingChallenge.Application.TVMaze.Queries;

namespace CodingChallenge.Api.Controllers;

[ApiController]
[Route("/api/shows")]
public class ShowController : Controller
{
    public static readonly ProblemDetails NotFoundResponse = new()
    {
        Type = "/api/shows/errors/not-found",
        Title = "TMS000",
        Detail = "Show not found",
        Status = StatusCodes.Status404NotFound
    };

    private readonly ISender _mediator;
    private readonly ILogger<ShowController> _logger;

    public ShowController(ISender sender, ILogger<ShowController> logger)
    {
        _mediator = sender;
        _logger = logger;
    }

    [HttpGet("{id:string}")]
    [ProducesResponseType(typeof(ShowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] string id,
        [FromServices] IMapper<TVMazeRecordDto, ShowResponse> mapper,
        CancellationToken cancellationToken)
    {
        var show = await _mediator.Send(new GetTVMazeItemByIndexQuery(id), cancellationToken);

        if (show == null)
        {
            _logger.LogWarning("Show with id {0} could not be found", id);
            return NotFound(NotFoundResponse);
        }

        return Ok(mapper.Map(show));
    }



    [HttpGet("getall/{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(ShowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetListAsync(
    [FromRoute] int pageNumber,
    [FromRoute] int pageSize,
    [FromServices] IMapper<PagedList<TVMazeRecordDto>, ShowListResponse> mapper,
    CancellationToken cancellationToken)
    {
        var shows = await _mediator.Send(new GetTVMazeItemsQuery(pageNumber, pageSize), cancellationToken);

        if (shows == null)
        {
            _logger.LogWarning("No shows found for pageNumber {0} and pageSize {1}", pageNumber, pageSize);
            return NotFound(NotFoundResponse);
        }

        return Ok(mapper.Map(shows));
    }
}
