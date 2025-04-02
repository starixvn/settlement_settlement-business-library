using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class WinLostServiceTests
{
    private readonly WinLostServiceImp service = new WinLostServiceImp();

    [Fact]
    public void GetWinLost_WhenWinLimitNotFound_ReturnsOriginalWinLost()
    {
        // Arrange
        var ticket = CreateTicket(custId: 1, winLost: 100);
        var winLimits = new List<WinLimit>
        {
            CreateWinLimit(custId: 2, maxWinLimit: 50)
        };

        // Act
        var result = service.GetWinLost(ticket, winLimits, isRecovery: false);

        // Assert
        Assert.False(result.isApplyMaxPayout);
        Assert.Equal(100, result.winLost);
    }

    [Theory]
    [InlineData(1000, 0.25, 500, true, 500)]
    [InlineData(1000, 0.15, 500, false, 1000)]  // Below quarter, doesn't apply
    [InlineData(400, 0.25, 500, false, 400)]    // Below limit, doesn't apply
    public void GetWinLost_WhenNotRecovery_AppliesModdsRules(
        decimal actualPayout,
        decimal modds,
        decimal maxWinLimit,
        bool expectedApplyPayout,
        decimal expectedWinLost)
    {
        // Arrange
        var ticket = CreateTicket(
            custId: 1,
            winLost: actualPayout,
            modds: modds,
            actualPayout: actualPayout);
        var winLimits = new List<WinLimit>
        {
            CreateWinLimit(custId: 1, maxWinLimit: maxWinLimit)
        };

        // Act
        var result = service.GetWinLost(ticket, winLimits, isRecovery: false);

        // Assert
        Assert.Equal(expectedApplyPayout, result.isApplyMaxPayout);
        Assert.Equal(expectedWinLost, result.winLost);
    }

    [Theory]
    [InlineData(Status.StatusWinlost.Win, 1000, 500, true, 500)]      // Win status, exceeds limit
    [InlineData(Status.StatusWinlost.HalfWin, 1000, 500, true, 500)]  // Half win status, exceeds limit
    [InlineData(Status.StatusWinlost.Loss, 1000, 500, false, 1000)]   // Loss status, doesn't apply
    [InlineData(Status.StatusWinlost.Win, 400, 500, false, 400)]      // Win status, below limit
    public void GetWinLost_WhenRecovery_AppliesStatusRules(
        byte statusWinLost,
        decimal actualPayout,
        decimal maxWinLimit,
        bool expectedApplyPayout,
        decimal expectedWinLost)
    {
        // Arrange
        var ticket = CreateTicket(
            custId: 1,
            winLost: actualPayout,
            statusWinLost: statusWinLost,
            actualPayout: actualPayout);
        var winLimits = new List<WinLimit>
        {
            CreateWinLimit(custId: 1, maxWinLimit: maxWinLimit)
        };

        // Act
        var result = service.GetWinLost(ticket, winLimits, isRecovery: true);

        // Assert
        Assert.Equal(expectedApplyPayout, result.isApplyMaxPayout);
        Assert.Equal(expectedWinLost, result.winLost);
    }

    [Fact]
    public void GetWinLost_WithNullWinLimits_RightExpected()
    {
        // Arrange
        var ticket = CreateTicket(custId: 1, winLost: 100);

        // Act
        var result = service.GetWinLost(ticket, [], isRecovery: true);

        // Assert
        Assert.False(result.isApplyMaxPayout);
        Assert.Equal(100, result.winLost);
    }

    private static Ticket CreateTicket(
        int custId,
        decimal? winLost = null,
        decimal modds = 0,
        byte? statusWinLost = null,
        decimal? actualPayout = null)
    {
        return new Ticket
        {
            CustId = custId,
            WinLost = winLost,
            Modds = modds,
            StatusWinLost = statusWinLost,
            ActualPayout = actualPayout
        };
    }

    private static WinLimit CreateWinLimit(int custId, decimal maxWinLimit)
    {
        return new WinLimit
        {
            CustId = custId,
            MaxWinLimit = maxWinLimit
        };
    }
}