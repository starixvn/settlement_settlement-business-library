using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class WinLostServiceTests
{
    private readonly WinLostService service = new WinLostServiceImp();

    [Fact]
    public void GetWinLost_IsExistedMaxPayout_ReturnsMaxWinLimitAndTrue()
    {
        const int expected = 5000;
        var ticket = new Ticket
        {
            CustId = 1,
            Modds = 0.5m,
            ActualPayout = 6000
        };

        var listWinLimit = new List<WinLimit>()
        {
            new()
            {
                CustId = 1,
                MaxWinLimit = expected
            }
        };

        var (isApplyMaxPayout, winLost) = service.GetWinLost(ticket, listWinLimit);

        Assert.True(isApplyMaxPayout);
        Assert.Equal(expected, winLost);
    }

    [Fact]
    public void GetWinLost_IsExistedMaxPayout_ReturnsWinLostAndFalse()
    {
        var ticket = new Ticket
        {
            CustId = 1,
            Modds = 0.5m,
            ActualPayout = 4000,
            WinLost = 3000
        };

        var listWinLimit = new List<WinLimit>()
        {
            new()
            {
                CustId = 1,
                MaxWinLimit = 5000
            }
        };

        var (isApplyMaxPayout, winLost) = service.GetWinLost(ticket, listWinLimit);

        const int expected = 3000;
        Assert.False(isApplyMaxPayout);
        Assert.Equal(expected, winLost);
    }
}