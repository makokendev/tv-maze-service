using CodingChallenge.Application.Interfaces;
using CodingChallenge.Application.TVMaze.Base;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Application.TVMaze.Commands.Burn;

public record AddScrapeTaskCommand(int StartIndex, int EndIndex, int TryCount) : TVMazeScrapeCommandBase(), 
    IRequest<AddScrapeTaskCommandResponse>;

public record AddScrapeTaskCommandResponse(int StartIndex, int EndIndex) : TVMazeScrapeCommandResponseBase();

public class AddScrapeTaskCommandHandler : IRequestHandler<AddScrapeTaskCommand, AddScrapeTaskCommandResponse>
{
    private readonly ITVMazeRecordRepository _repo;
    private readonly ILogger _logger;

    public AddScrapeTaskCommandHandler(ITVMazeRecordRepository repo, ILogger logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<AddScrapeTaskCommandResponse> Handle(AddScrapeTaskCommand request, CancellationToken cancellationToken)
    {
        await _repo.AddScrapeTaskAsync(request);
        var response = new AddScrapeTaskCommandResponse(request.StartIndex,request.EndIndex);
        return response;
    }

}
