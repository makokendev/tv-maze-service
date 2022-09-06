using AutoMapper;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Domain.Entities.TvMaze;
using MediatR;

namespace CodingChallenge.Application.TVMaze.Queries;

public record GetTVMazeItemsQuery(int PageNumber, int PageSize) : IRequest<PagedList<TVMazeRecordDto>>;

public class GetTVMazeItemsQueryHandler : IRequestHandler<GetTVMazeItemsQuery, PagedList<TVMazeRecordDto>>
{
    private readonly ITVMazeRecordRepository repo;
    private readonly IMapper _mapper;

    public GetTVMazeItemsQueryHandler(ITVMazeRecordRepository context, IMapper mapper)
    {
        repo = context;
        _mapper = mapper;
    }

    public async Task<PagedList<TVMazeRecordDto>> Handle(GetTVMazeItemsQuery request, CancellationToken cancellationToken)
    {
        var responseEntity = await repo.GetItemListAsync(request.PageNumber, request.PageSize);
        var list = responseEntity.ToList();
        var dtList = _mapper.Map<List<TVMazeRecordEntity>,List<TVMazeRecordDto>>(list);
        return new PagedList<TVMazeRecordDto>(dtList,responseEntity.ToList().Count,request.PageNumber,request.PageSize);
    }
}
