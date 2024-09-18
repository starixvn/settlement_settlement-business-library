using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class WinLostByModdsServiceTests
{
    private readonly WinLostByModdsService service = new WinLostByModdsServiceImp();

    [Fact]
    public void WinLostMaxPayout_ModdsLessThanQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.1m,
        };

        const int maxWinLost = 5000;

        var actual = service.WinLostMaxPayout(ticket, maxWinLost);

        const int expectedWinlost = 5000;

        Assert.Equal(expectedWinlost, actual.WinLost);
    }

    [Fact]
    public void WinLostMaxPayout_ModdsGreaterThanQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.5m,
            ActualStake = 0.1m,
            APositionTaking = 0.2m,
            MPositionTaking = 0.3m,
            SPositionTaking = 0.4m
        };

        const int maxWinLost = 1000;

        var actual = service.WinLostMaxPayout(ticket, maxWinLost);

        var expectedWinlost = new Winlost
        {
            WinLost = 1000,
            AWinLost = -199.98m,
            MWinLost = -299.97m,
            SWinLost = -399.96m
        };

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void WinLostGreaterThanQuarter_IsExistedDeadHeat_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            DeadHeat = 1,
            Modds = 0.5m,
            Stake = 100,
            APositionTaking = 0.2m,
            MPositionTaking = 0.3m,
            SPositionTaking = 0.4m
        };

        const decimal odds = 0.1m;

        var actual = service.WinLostGreaterThanQuarter(ticket, odds);

        var expectedWinlost = new Winlost
        {
            WinLost = 110.0m,
            AWinLost = -2.00m,
            MWinLost = -3.00m,
            SWinLost = -4.00m
        };

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void WinLostGreaterThanQuarter_IsNotExistedDeadHeat_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.5m,
            Stake = 100,
            ActualStake = 0.1m,
            APositionTaking = 0.2m,
            MPositionTaking = 0.3m,
            SPositionTaking = 0.4m
        };

        const decimal odds = 0.1m;

        var actual = service.WinLostGreaterThanQuarter(ticket, odds);

        var expectedWinlost = new Winlost
        {
            WinLost = 10.1m,
            AWinLost = -2.00m,
            MWinLost = -3.00m,
            SWinLost = -4.00m
        };

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void WinLostEqualQuarter_Always_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.25m,
            Stake = 100,
            ActualStake = 0.1m,
            APositionTaking = 0.2m,
            MPositionTaking = 0.3m,
            SPositionTaking = 0.4m
        };

        const decimal odds = 0.1m;

        var actual = service.WinLostEqualQuarter(ticket, odds);

        var expectedWinlost = new Winlost
        {
            WinLost = 5.10m,
            AWinLost = -1.000m,
            MWinLost = -1.500m,
            SWinLost = -2.000m
        };

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void WinLostEqualNegativeQuarter_Always_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = -0.25m,
            Stake = 100,
            ActualStake = 0.1m,
            APositionTaking = 0.2m,
            MPositionTaking = 0.3m,
            SPositionTaking = 0.4m
        };

        var actual = service.WinLostEqualNegativeQuarter(ticket);

        var expectedWinlost = new Winlost
        {
            WinLost = 0.05m,
            AWinLost = 0.010m,
            MWinLost = 0.015m,
            SWinLost = 0.020m
        };

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void WinLostLowerThanNegativeQuarter_Always_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = -0.35m,
            Stake = 100,
            ActualStake = 0.1m,
            APositionTaking = 0.2m,
            MPositionTaking = 0.3m,
            SPositionTaking = 0.4m
        };

        var actual = service.WinLostLowerThanNegativeQuarter(ticket);

        var expectedWinlost = new Winlost
        {
            WinLost = 0,
            AWinLost = 0.02m,
            MWinLost = 0.03m,
            SWinLost = 0.04m
        };

        AssertExtension.AreEqual(expectedWinlost, actual);
    }
}