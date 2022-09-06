using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using CodingChallenge.Application.AutoMapper;
using CodingChallenge.Domain.Base;
using CodingChallenge.Domain.Entities;
using CodingChallenge.Domain.Entities.TvMaze;

namespace CodingChallenge.Infrastructure.Persistence.TVMazeRecord;

public class TVMazeRecordDataModelTVMazeRecordEntityResolver : IValueResolver<TVMazeRecordDataModel, TVMazeRecordEntity, TvMazeRecord>
{
    public TvMazeRecord Resolve(TVMazeRecordDataModel source, TVMazeRecordEntity destination, TvMazeRecord member, ResolutionContext context) 
        => new(Convert.ToInt32(source.TVMazeIndex));
}
public class TVMazeRecordDataModel : AuditableEntity, IMapFrom<TVMazeRecordEntity>
{

    //[DynamoDBRangeKey]
    [DynamoDBHashKey]
    public int TVMazeIndex { get; set; } = 0;
    [DynamoDBProperty]
    public string TVMazeType { get; set; } = string.Empty;
    [DynamoDBProperty]
    public List<TVMazeCastItem> CastList { get; set; } = new List<TVMazeCastItem>();


    public void Mapping(Profile profile)
    {
        profile.CreateMap<TVMazeRecordEntity, TVMazeRecordDataModel>()
            .ForMember(d => d.TVMazeIndex, opt => opt.MapFrom(s => s.Index.ToString()))
            .ForMember(d => d.TVMazeType, opt => opt.MapFrom(s => s.ProductionType));
        profile.CreateMap<TVMazeRecordDataModel, TVMazeRecordEntity>()
            .ForMember(d => d.Index, opt => opt.MapFrom(s => s.TVMazeIndex))
            .ForMember(d => d.ProductionType, opt => opt.MapFrom(s => s.TVMazeType));
            //.ForMember(d => d.CastList, opt => opt.MapFrom(s=>JsonConvert.SerializeObject(s.CastList)));
    }
}


