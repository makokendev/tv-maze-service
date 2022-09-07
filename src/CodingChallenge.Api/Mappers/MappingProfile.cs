using AutoMapper;
using CodingChallenge.Api.Models.Responses;
using CodingChallenge.Application.AutoMapper;
using CodingChallenge.Application.TVMaze.Queries;
using CodingChallenge.Domain.Entities;

namespace CodingChallenge.Api.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        typeof(MappingProfile).Assembly.ApplyMappingsFromAssembly(this);

        SetupMapping();
    }

    private void SetupMapping()
    {
        CreateMap<PagedList<TVMazeRecordDto>, PagedList<ShowResponse>>();

        CreateMap<TVMazeRecordDto, ShowResponse>()
            .ForMember(dest => dest.Id, a => a.MapFrom(o => o.Index))
            .ForMember(dest => dest.Cast, a => a.MapFrom(o => o.CastList));

        CreateMap<TVMazeCastItem, CastMemberResponse>()
            .ForMember(dest => dest.Id, a => a.MapFrom(o => o.person.id))
            .ForMember(dest => dest.Name, a => a.MapFrom(o => o.person.name))
            .ForMember(dest => dest.BirthDate, a => a.MapFrom(o => o.person.birthday));
    }
}
