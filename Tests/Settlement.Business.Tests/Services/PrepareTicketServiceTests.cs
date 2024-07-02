using NSubstitute;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Tests.Services;

public class PrepareTicketServiceTests
{
    private readonly CommissionService commissionService;

    private readonly WinLostByModdsService winLostByModdsService;

    private readonly WinLostService winLostService;

    private readonly PrepareTicketService service;

    public PrepareTicketServiceTests()
    {
        commissionService = Substitute.For<CommissionService>();
        winLostByModdsService = Substitute.For<WinLostByModdsService>();
        winLostService = Substitute.For<WinLostService>();

        service = new PrepareTicketServiceImp(commissionService, winLostByModdsService, winLostService);
    }

    [Fact]
    public void CreateValueIsUpdated_Always_ReturnsTrue()
    {
        var ticket = new Ticket
        {
            IsMarked = false,
            StatusId = StatusId.Running
        };
        var winLimit = new List<WinLimit>();
        var actual = service.CreateValue(ticket, winLimit);

        Assert.True(actual.IsUpdated);
    }

    [Fact]
    public void CreateValueStatusWinLost_MOddsGreaterThanZero_ReturnsWin()
    {
        var ticket = new Ticket
        {
            Modds = 1
        };
        var winLimit = new List<WinLimit>();
        var actual = service.CreateValue(ticket, winLimit);

        const byte expected = StatusWinlost.Win;
        Assert.Equal(expected, actual.StatusWinLost);
    }

    [Fact]
    public void CreateValueStatusWinLost_MOddsLessThanZero_ReturnsLost()
    {
        var ticket = new Ticket
        {
            Modds = -1
        };
        var winLimit = new List<WinLimit>();
        var actual = service.CreateValue(ticket, winLimit);

        const byte expected = StatusWinlost.Lost;
        Assert.Equal(expected, actual.StatusWinLost);
    }

    [Fact]
    public void CreateValueExtraInfo_NotApplyMaxPayout_ReturnsCorrectExtraInfo()
    {
        var ticket = new Ticket
        {
            Odds = -1,
            ExtraInfo = "Test"
        };

        var winLimit = new List<WinLimit>();
        winLostService.GetWinLost(Arg.Any<Ticket>(), Arg.Any<List<WinLimit>>()).Returns((false, null));
        var actual = service.CreateValue(ticket, winLimit);

        const string expected = "Test";
        Assert.Equal(expected, actual.ExtraInfo);
    }

    [Fact]
    public void CreateValueExtraInfo_ApplyMaxPayoutAndExtraInfoIsEmpty_ReturnsCorrectExtraValue()
    {
        var ticket = new Ticket
        {
            Odds = -1,
            ExtraInfo = string.Empty,
            ActualPayout = 0.5m
        };

        var winLimit = new List<WinLimit>();
        winLostService.GetWinLost(Arg.Any<Ticket>(), Arg.Any<List<WinLimit>>()).Returns((true, null));
        var actual = service.CreateValue(ticket, winLimit);

        const string expected = "<l255 apo=\"0.5\" />";
        Assert.Equal(expected, actual.ExtraInfo);
    }

    [Fact]
    public void CreateValueExtraInfo_ApplyMaxPayoutAndExtraInfoIsNotEmpty_ReturnsCorrectExtraValue()
    {
        var ticket = new Ticket
        {
            Odds = -1,
            ExtraInfo = "Test",
            ActualPayout = 0.5m
        };

        var winLimit = new List<WinLimit>();
        winLostService.GetWinLost(Arg.Any<Ticket>(), Arg.Any<List<WinLimit>>()).Returns((true, null));
        var actual = service.CreateValue(ticket, winLimit);

        const string expected = "Test<l255 apo=\"0.5\" />";
        Assert.Equal(expected, actual.ExtraInfo);
    }

    [Fact]
    public void CreateValueOdds_OddsLessThanZero_ReturnsNumberOne()
    {
        var ticket = new Ticket
        {
            Odds = -1,
        };

        var winLimit = new List<WinLimit>();

        var actual = service.CreateValue(ticket, winLimit);

        Assert.True(actual.Odds == 1);
    }

    [Fact]
    public void CreateValueOdds_OddsGreaterThanZeroAndOddsTypeNotEqualOne_ReturnsOdds()
    {
        var ticket = new Ticket
        {
            Odds = 2,
            OddsType = 0
        };

        var winLimit = new List<WinLimit>();

        var actual = service.CreateValue(ticket, winLimit);

        Assert.True(actual.Odds == ticket.Odds);
    }

    [Fact]
    public void CreateValueOdds_OddsGreaterThanZeroAndOddsTypeEqualOne_ReturnsExpectedValue()
    {
        var ticket = new Ticket
        {
            Odds = 2,
            OddsType = 1,
            BetType = BetTypes.FirstHalfHomeTeamOddEven,
            Modds = 2,
        };

        var winLimit = new List<WinLimit>();

        var actual = service.CreateValue(ticket, winLimit);
        var expected = ticket.Modds - 1;
        Assert.Equal(expected, actual.Odds);
    }

    [Fact]
    public void CreateValueOdds_OddsGreaterThanZeroAndOddsTypeEqualOneAndModdsGreaterThanQuarter_ReturnsExpectedValue()
    {
        var ticket = new Ticket
        {
            Odds = 2,
            OddsType = 1,
            BetType = BetTypes.OneXTwoBackLay,
            Modds = 1,
            BetTeam = "al"
        };

        var winLimit = new List<WinLimit>();

        var actual = service.CreateValue(ticket, winLimit);
        const int expected = 1;
        Assert.Equal(expected, actual.Odds);
    }

    [Fact]
    public void CreateValueOdds_OddsGreaterThanZeroAndOddsTypeEqualOneAndModdsLessThanQuarter_ReturnsExpectedValue()
    {
        var ticket = new Ticket
        {
            Odds = 2,
            OddsType = 1,
            BetType = BetTypes.OneXTwoBackLay,
            Modds = 0.1m,
        };

        var winLimit = new List<WinLimit>();

        var actual = service.CreateValue(ticket, winLimit);
        var expected = ticket.Odds - 1;
        Assert.Equal(expected, actual.Odds);
    }

    [Fact]
    public void CreateValueIsUpdateSettlementTime_ChangeStatusId_ReturnsTrue()
    {
        var ticket = new Ticket
        {
            Modds = 0.1m,
            StatusId = StatusId.Running,
            IsUpdateSettlementTime = false
        };

        var winLimit = new List<WinLimit>();

        var actual = service.CreateValue(ticket, winLimit);

        Assert.True(actual.IsUpdateSettlementTime);
    }

    [Fact]
    public void CreateValueIsUpdateSettlementTime_NotChangeStatusId_ReturnsFalse()
    {
        var ticket = new Ticket
        {
            Modds = 0.1m,
            WinLost = 0,
            StatusId = StatusId.Won
        };

        var winLimit = new List<WinLimit>();

        var actual = service.CreateValue(ticket, winLimit);

        Assert.False(actual.IsUpdateSettlementTime);
    }

    [Fact]
    public void CreateCommission_ModdsLessThanNegativeQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = -0.35m
        };

        var expectedCommission = new Commission
        {
            PlayerComm = 0,
            Comm = 0.02m,
            AComm = 0.03m,
            SComm = 0.04m
        };

        commissionService.ModdsLowerThanNegativeQuarter(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(expectedCommission);
        var actual = service.CreateCommission(ticket, new PrepareTicket());

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void CreateCommission_ModdsEqualNegativeQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = -0.25m
        };

        var expectedCommission = new Commission
        {
            PlayerComm = 0,
            Comm = 0.02m,
            AComm = 0.03m,
            SComm = 0.04m
        };

        commissionService.ModdsEqualNegativeQuarter(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(expectedCommission);
        var actual = service.CreateCommission(ticket, new PrepareTicket());

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void CreateCommission_ModdsEqualQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.25m
        };

        var expectedCommission = new Commission
        {
            PlayerComm = 0,
            Comm = 0.02m,
            AComm = 0.03m,
            SComm = 0.04m
        };

        commissionService.ModdsEqualQuarter(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(expectedCommission);
        var actual = service.CreateCommission(ticket, new PrepareTicket());

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void CreateCommission_ModdsGreaterThanQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.35m
        };

        var expectedCommission = new Commission
        {
            PlayerComm = 0,
            Comm = 0.02m,
            AComm = 0.03m,
            SComm = 0.04m
        };

        commissionService.ModdsHigherThanQuarter(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(expectedCommission);
        var actual = service.CreateCommission(ticket, new PrepareTicket());

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void CreateCommission_ModdsLessThanQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.15m,
        };

        var expectedCommission = new Commission();

        var actual = service.CreateCommission(ticket, new PrepareTicket());

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void CreateWinLost_MaxPayoutIsTrue_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket();

        var prepareTicket = new PrepareTicket
        {
            WinLost = 1000,
            IsApplyMaxPayout = true
        };
        var expectedWinlost = new Winlost
        {
            WinLost = 1000,
            AWinLost = -199.98m,
            MWinLost = -299.97m,
            SWinLost = -399.96m
        };

        winLostByModdsService.WinLostMaxPayout(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(expectedWinlost);
        var actual = service.CreateWinLost(ticket, prepareTicket);

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void CreateWinLost_ModdsLessThanNegativeQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = -0.35m
        };

        var prepareTicket = new PrepareTicket
        {
            WinLost = 1000,
            IsApplyMaxPayout = false
        };
        var expectedWinlost = new Winlost
        {
            WinLost = 0,
            AWinLost = 0.02m,
            MWinLost = 0.03m,
            SWinLost = 0.04m
        };

        winLostByModdsService.WinLostLowerThanNegativeQuarter(Arg.Any<Ticket>()).Returns(expectedWinlost);
        var actual = service.CreateWinLost(ticket, prepareTicket);

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void CreateWinLost_ModdsEqualNegativeQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = -0.25m
        };

        var prepareTicket = new PrepareTicket
        {
            WinLost = 1000,
            IsApplyMaxPayout = false
        };
        var expectedWinlost = new Winlost
        {
            WinLost = 0.05m,
            AWinLost = 0.010m,
            MWinLost = 0.015m,
            SWinLost = 0.020m
        };

        winLostByModdsService.WinLostEqualNegativeQuarter(Arg.Any<Ticket>()).Returns(expectedWinlost);
        var actual = service.CreateWinLost(ticket, prepareTicket);

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void CreateWinLost_ModdsEqualQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.25m
        };

        var prepareTicket = new PrepareTicket
        {
            WinLost = 1000,
            IsApplyMaxPayout = false
        };
        var expectedWinlost = new Winlost
        {
            WinLost = 5.10m,
            AWinLost = -1.000m,
            MWinLost = -1.500m,
            SWinLost = -2.000m
        };

        winLostByModdsService.WinLostEqualQuarter(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(expectedWinlost);
        var actual = service.CreateWinLost(ticket, prepareTicket);

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void CreateWinLost_ModdsGreaterThanQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.35m
        };

        var prepareTicket = new PrepareTicket
        {
            WinLost = 1000,
            IsApplyMaxPayout = false
        };
        var expectedWinlost = new Winlost
        {
            WinLost = 10.1m,
            AWinLost = -2.00m,
            MWinLost = -3.00m,
            SWinLost = -4.00m
        };

        winLostByModdsService.WinLostGreaterThanQuarter(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(expectedWinlost);
        var actual = service.CreateWinLost(ticket, prepareTicket);

        AssertExtension.AreEqual(expectedWinlost, actual);
    }

    [Fact]
    public void CreateWinLost_ModdsLessThanQuarter_ReturnsExpectedWinLost()
    {
        var ticket = new Ticket
        {
            Modds = 0.15m,
            ActualStake = 111
        };

        var prepareTicket = new PrepareTicket
        {
            WinLost = 1000,
            IsApplyMaxPayout = false
        };
        var expectedWinlost = new Winlost
        {
            WinLost = 111,
        };

        var actual = service.CreateWinLost(ticket, prepareTicket);

        AssertExtension.AreEqual(expectedWinlost, actual);
    }
}