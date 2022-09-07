using AutoMapper;
using CodingChallenge.Api.Models.Responses;
using CodingChallenge.Application.AutoMapper;
using CodingChallenge.Application.TVMaze.Queries;

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
        CreateMap<TVMazeRecordDto, ShowResponse>()
            .ForMember(dest => dest.Id, a => a.MapFrom(o => o.Index));
        //        .ForMember(dest => dest.Name, a => a.MapFrom(o => o.Na))
        //        .ForMember(dest => dest.Wallet, a => a.MapFrom<MindCommandTVMazeRecordEntityResolver>());
        //}

    }

}
