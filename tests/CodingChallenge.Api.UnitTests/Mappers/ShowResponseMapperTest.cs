//using AutoFixture;
//using FluentAssertions;
//using TvMaze.Service.Api.Mappers;
//using TvMaze.Service.Infrastructure.Repositories.Show.ViewModels;
//using Xunit;

//namespace CodingChallenge.Api.UnitTests.Mappers;

//public class ShowResponseMapperTest
//{
//    private readonly Fixture _fixture;
//    private readonly ShowResponseMapper _mapper;

//    public ShowResponseMapperTest()
//    {
//        _fixture = new();
//        _mapper = new();
//    }

//    [Fact]
//    public void Mapper()
//    {
//        var source = _fixture.Build<ShowViewModel>().Create();
//        var destiny = _mapper.Map(source);

//        destiny.Id.Should().Be(source.Id);
//        destiny.Name.Should().Be(source.Name);
//        destiny.Cast.Should().HaveCount(source.Cast.Count());
//    }
//}
