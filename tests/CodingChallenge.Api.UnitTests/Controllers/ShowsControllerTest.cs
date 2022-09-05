//using AutoFixture;
//using CodingChallenge.Api.Controllers;
//using CodingChallenge.Application.TVMaze.Queries;
//using FluentAssertions;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using NSubstitute;
//using NSubstitute.ReturnsExtensions;

//using Xunit;
//using static CodingChallenge.Api.Controllers.ShowController;

//namespace TvMaze.Service.Api.Tests.Controllers
//{
//    public class ShowsControllerTest
//    {
//        private readonly Fixture _fixture;
//        private readonly ISender _mediator;
//        private readonly IServiceProvider _provider;
//        private readonly ShowController _controller;

//        public ShowsControllerTest()
//        {
//            _fixture = new();
//            _mediator = Substitute.For<ISender>();
//            _provider = Substitute.For<IServiceProvider>();
//            _controller = new(_mediator, Substitute.For<ILogger<ShowController>>());
//        }

//        #region GetAsync

//        [Fact]
//        public async Task GetAsync_Should_ReturnNotFound_When_ShowNotExists()
//        {
//            var id = _fixture.Create<int>();
//            _mediator.Send(new GetTVMazeItemByIndexQuery(Arg.Any<string>()), Arg.Any<CancellationToken>())
//                .ReturnsNull();

//            var mapper = Substitute.For<IMapper<ShowViewModel, ShowResponse>>();

//            var action = await _controller.GetAsync(id, mapper, CancellationToken.None);

//            action.Should().BeOfType<NotFoundObjectResult>()
//                .Subject.Value.Should().Be(NotFoundResponse);

//            await _mediator
//                .Received(1)
//                .GetShowByIdAsync(id, Arg.Any<CancellationToken>());
//        }

//        [Fact]
//        public async Task GetAsync()
//        {
//            var id = _fixture.Create<int>();

//            var viewModel = _fixture.Create<ShowViewModel>();

//            _mediator.GetShowByIdAsync(id, Arg.Any<CancellationToken>())
//                .Returns(viewModel);

//            var response = _fixture.Create<ShowResponse>();
//            var mapper = Substitute.For<IMapper<ShowViewModel, ShowResponse>>();
//            mapper.Map(viewModel)
//                .Returns(response);

//            var action = await _controller.GetAsync(id, mapper, CancellationToken.None);

//            action.Should().BeOfType<OkObjectResult>()
//                .Subject.Value.Should().BeOfType<ShowResponse>();

//            await _mediator
//                .Received(1)
//                .GetShowByIdAsync(id, Arg.Any<CancellationToken>());

//            mapper
//                .Received(1)
//                .Map(viewModel);

//        }

//        #endregion

//        #region GetListAsync

//        [Fact]
//        public async Task GetListAsync_Should_ReturnNotFound_When_ShowNotExists()
//        {
//            int pageNum = _fixture.Create<int>(), pageSize = _fixture.Create<int>();

//            _mediator.GetShowListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
//                .ReturnsNull();

//            var mapper = Substitute.For<IMapper<PagedList<ShowViewModel>, ShowListResponse>>();

//            var action = await _controller.GetListAsync(
//                pageNum, pageSize, mapper, CancellationToken.None);

//            action.Should().BeOfType<NotFoundObjectResult>()
//                .Subject.Value.Should().Be(NotFoundResponse);

//            await _mediator
//                .Received(1)
//                .GetShowListAsync(pageNum, pageSize, Arg.Any<CancellationToken>());
//        }

//        [Fact]
//        public async Task GetListAsync()
//        {
//            int pageNum = _fixture.Create<int>(), pageSize = _fixture.Create<int>();

//            var viewModel = _fixture.Create<PagedList<ShowViewModel>>();

//            _mediator.GetShowListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
//                .Returns(viewModel);

//            var response = _fixture.Create<ShowListResponse>();
//            var mapper = Substitute.For<IMapper<PagedList<ShowViewModel>, ShowListResponse>>();
//            mapper.Map(viewModel)
//                .Returns(response);

//            var action = await _controller.GetListAsync(pageNum, pageSize, mapper, CancellationToken.None);

//            action.Should().BeOfType<OkObjectResult>()
//                .Subject.Value.Should().BeOfType<ShowListResponse>();

//            await _mediator
//                .Received(1)
//                .GetShowListAsync(pageNum, pageSize, Arg.Any<CancellationToken>());

//            mapper
//                .Received(1)
//                .Map(viewModel);
//        }

//        #endregion
//    }
//}