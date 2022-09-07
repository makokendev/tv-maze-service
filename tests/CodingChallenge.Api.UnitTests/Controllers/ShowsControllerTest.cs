using AutoFixture;
using AutoMapper;
using CodingChallenge.Api.Controllers;
using CodingChallenge.Api.Models.Responses;
using CodingChallenge.Application.TVMaze.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Xunit;
using static CodingChallenge.Api.Controllers.ShowController;

namespace TvMaze.Service.Api.Tests.Controllers
{
    public class ShowsControllerTest
    {
        private readonly Fixture _fixture;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _provider;
        private readonly ShowController _controller;

        public ShowsControllerTest()
        {
            _fixture = new();
            _mediator = Substitute.For<ISender>();
            _provider = Substitute.For<IServiceProvider>();
            _mapper = Substitute.For<IMapper>();

            _controller = new(_mediator, _mapper, Substitute.For<ILogger<ShowController>>());
        }

        #region GetAsync

        [Fact]
        public async Task GetAsync_Should_ReturnNotFound_When_ShowNotExists()
        {
            var id = _fixture.Create<string>();

            _mediator.Send(Arg.Any<GetTVMazeItemByIndexQuery>(), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var action = await _controller.GetAsync(id, CancellationToken.None);

            action.Should().BeOfType<NotFoundObjectResult>()
                .Subject.Value.Should().Be(NotFoundResponse);

            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTVMazeItemByIndexQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAsync()
        {
            var id = _fixture.Create<string>();

            var viewModel = _fixture.Create<TVMazeRecordDto>();

            _mediator.Send(Arg.Any<GetTVMazeItemByIndexQuery>(), Arg.Any<CancellationToken>())
                .Returns(viewModel);

            var response = _fixture.Create<ShowResponse>();

            _mapper.Map<ShowResponse>(viewModel).Returns(response);

            var action = await _controller.GetAsync(id, CancellationToken.None);

            action.Should().BeOfType<OkObjectResult>()
                .Subject.Value.Should().BeOfType<ShowResponse>();

            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTVMazeItemByIndexQuery>(), Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<ShowResponse>(viewModel);

        }

        #endregion

        #region GetListAsync

        [Fact]
        public async Task GetListAsync_Should_ReturnNotFound_When_ShowNotExists()
        {
            var paginationToken = _fixture.Create<string>();
            var pageSize = _fixture.Create<int>();

            _mediator.Send(Arg.Any<GetTVMazeItemsQuery>(), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var action = await _controller.GetListAsync(
                pageSize, paginationToken, CancellationToken.None);

            action.Should().BeOfType<NotFoundObjectResult>()
                .Subject.Value.Should().Be(NotFoundResponse);

            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTVMazeItemsQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetListAsync()
        {
            var paginationToken = _fixture.Create<string>();
            var pageSize = _fixture.Create<int>();

            var viewModel = _fixture.Create<PagedList<TVMazeRecordDto>>();

            _mediator.Send(Arg.Any<GetTVMazeItemsQuery>(), Arg.Any<CancellationToken>())
                .Returns(viewModel);

            var response = _fixture.Create<PagedList<ShowResponse>>();

            _mapper.Map<PagedList<ShowResponse>>(viewModel).Returns(response);

            var action = await _controller.GetListAsync(
                pageSize, paginationToken, CancellationToken.None);

            action.Should().BeOfType<OkObjectResult>()
                .Subject.Value.Should().BeOfType<PagedList<ShowResponse>>();

            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTVMazeItemsQuery>(), Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<PagedList<ShowResponse>>(viewModel);
        }

        #endregion
    }
}