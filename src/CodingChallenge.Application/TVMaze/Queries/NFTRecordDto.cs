using AutoMapper;
using CodingChallenge.Application.AutoMapper;
using CodingChallenge.Domain.Entities;
using CodingChallenge.Domain.Entities.TvMaze;

namespace CodingChallenge.Application.TVMaze.Queries;

public class TVMazeRecordDto : IMapFrom<TVMazeRecordEntity>
{
    public int Index { get; set; }
    public string ProductionType { get; set; } = string.Empty;
    public IEnumerable<TVMazeCastItem> CastList { get; set; } = Enumerable.Empty<TVMazeCastItem>();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TVMazeRecordEntity, TVMazeRecordDto>();

    }
}
