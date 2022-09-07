using System.Runtime.Serialization;
using AutoMapper;
using CodingChallenge.Api.Mappers;
using CodingChallenge.Api.Models.Responses;
using CodingChallenge.Application.TVMaze.Queries;
using CodingChallenge.Domain.Entities;
using Xunit;

namespace CodingChallenge.Api.UnitTests.Mappers;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();
    }

    [Theory]
    [InlineData(typeof(PagedList<TVMazeRecordDto>), typeof(PagedList<ShowResponse>))]
    [InlineData(typeof(TVMazeRecordDto), typeof(ShowResponse))]
    [InlineData(typeof(TVMazeCastItem), typeof(CastMemberResponse))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);
        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}
