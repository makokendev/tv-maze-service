using Xunit;

namespace CodingChallenge.Application.IntegrationTests.NFT.Queries;

public class GetByTokenIdQueryTests : CQRSTestBase
{
    [Fact]
    public async Task GetItemByIdQueryShouldSucceed()
    {
        //var scrapeResponse = await SendScrapeCommandAsync(1);
        //var getIndex = await GetItemByIndexCommandAsync(scrapeResponse.Index);
        var getList = await GetItemListAsync(5);

        Assert.NotEqual(getList.PageSize.ToString(),getList.PaginationToken);
    }
}
