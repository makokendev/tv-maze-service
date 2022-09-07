using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly ILogger<ShowController> _logger;

    public ShowController(ISender sender, IMapper mapper, ILogger<ShowController> logger)
    {
        _mediator = sender;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ShowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var show = await _mediator.Send(new GetTVMazeItemByIndexQuery(id), cancellationToken);

        if (show == null)
        {
            _logger.LogWarning("Show with id {0} could not be found", id);
            return NotFound(NotFoundResponse);
        }

        return Ok(_mapper.Map<ShowResponse>(show));
    }

    [HttpGet("getall/{pageSize:int}")]
    [ProducesResponseType(typeof(PagedList<ShowResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetListAsync(
        [FromRoute] int pageSize,
        [FromQuery] string? paginationToken=null,
        CancellationToken cancellationToken=default(CancellationToken))
    {
        var shows = await _mediator.Send(new GetTVMazeItemsQuery(pageSize, paginationToken), cancellationToken);

        if (shows == null)
        {
            _logger.LogWarning("No shows found for pageNumber {0} and pageSize {1}", paginationToken, pageSize);
            return NotFound(NotFoundResponse);
        }

        return Ok(_mapper.Map<PagedList<ShowResponse>>(shows));
    }
}