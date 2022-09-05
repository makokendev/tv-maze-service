﻿using Xunit;

namespace CodingChallenge.Application.IntegrationTests.NFT.Queries;

public class GetByTokenIdQueryTests : CQRSTestBase
{
    [Fact]
    public async Task GetItemByIdQueryShouldSucceed()
    {
        var scrapeResponse = await SendScrapeCommandAsync(1);
        var getIndex = await GetItemByIndexCommandAsync(scrapeResponse.Index);

        Assert.Equal(scrapeResponse.Index,getIndex.Index);
    }
}
