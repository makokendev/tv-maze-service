using AutoMapper;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Domain.Entities.TvMaze;
using MediatR;

namespace CodingChallenge.Application.TVMaze.Queries;

public record GetTVMazeItemByIndexQuery(string Index) : IRequest<TVMazeRecordDto>;

public class GetTVMazeItemByIndexQueryHandler : IRequestHandler<GetTVMazeItemByIndexQuery, TVMazeRecordDto>
{
    private readonly ITVMazeRecordRepository _repo;
    private readonly IMapper _mapper;

    public GetTVMazeItemByIndexQueryHandler(ITVMazeRecordRepository context, IMapper mapper)
    {
        _repo = context;
        _mapper = mapper;
    }

    public async Task<TVMazeRecordDto> Handle(GetTVMazeItemByIndexQuery request, CancellationToken cancellationToken)
    {
        var responseEntity = await _repo.GetByIndexAsync(request.Index);
        return _mapper.Map<TVMazeRecordEntity, TVMazeRecordDto>(responseEntity);
    }
}
