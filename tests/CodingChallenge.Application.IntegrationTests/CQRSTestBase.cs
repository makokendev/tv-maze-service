using System.Numerics;
using CodingChallenge.Application.TVMaze.Base;
using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Application.TVMaze.Commands.Scrape;
using CodingChallenge.Application.TVMaze.Queries;

namespace CodingChallenge.Application.IntegrationTests;

public class CQRSTestBase : TestBase
{
    public CQRSTestBase()
    {
        Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");
        Init();
    }

    public string GenerateBigIntegerHexadecimal()
    {
        return $"{ValidationExtensions.HexadecimalPrefix}{new BigInteger(new Random().NextInt64()).ToString("X40")}";
    }

    public async Task<ScrapeCommandResponse> SendScrapeCommandAsync(int index = 0)
    {
        var command = new ScrapeCommand(index);
        return await Sender!.Send(command);
    }

    public async Task<TVMazeRecordDto> GetItemByIndexCommandAsync(int index = 0)
    {
        var command = new GetTVMazeItemByIndexQuery(index.ToString());
        return await Sender!.Send(command);
    }
    public async Task<PagedList<TVMazeRecordDto>> GetItemListAsync(int index = 0)
    {
        var command = new GetTVMazeItemsQuery(index,null);
        return await Sender!.Send(command);
    }

    public async Task<AddScrapeTaskCommandResponse> AddScrapeTaskCommandAsync(int startIndex, int endIndex)
    {
        var command = new AddScrapeTaskCommand(startIndex, endIndex, 0);
        return await Sender!.Send(command);
    }
}
