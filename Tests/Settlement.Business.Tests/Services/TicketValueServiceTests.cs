using Moq;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Tests.Services;

public class TicketValueServiceTests
{
    private readonly Mock<PrepareTicketService> _prepareTicketServiceMock;
    private readonly TicketValueService _service;

    public TicketValueServiceTests()
    {
        _prepareTicketServiceMock = new Mock<PrepareTicketService>();
        _service = new TicketValueServiceImp(_prepareTicketServiceMock.Object);
    }

    #region GetActualPayout Tests

    [Theory]
    [InlineData(1.5, 2.0, 1, 100, 50, 150)]    // Greater than ModdsQuarter
    [InlineData(0.25, 2.0, 1, 100, 50, 125)]   // Equal to ModdsQuarter
    [InlineData(1.5, 3.0, 2, 100, 50, 250)]    // OddsType 2
    public void GetActualPayout_WithValidModds_ReturnsExpectedValue(
        decimal modds, decimal odds, short oddsType,
        decimal actualStake, decimal stake, decimal expected)
    {
        // Act
        var result = _service.GetActualPayout(modds, odds, oddsType, actualStake, stake);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-1.0, 2.0, 1, 100, 50)]    // Negative modds
    [InlineData(-0.5, 2.0, 1, 100, 50)]    // Less than ModdsQuarter
    public void GetActualPayout_WithInvalidModds_ReturnsNull(
        decimal modds, decimal odds, short oddsType,
        decimal actualStake, decimal stake)
    {
        // Act
        var result = _service.GetActualPayout(modds, odds, oddsType, actualStake, stake);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(-2.0, 1, 100, 50, 150)]    // Negative odds becomes 1
    [InlineData(2.0, 1, 100, 50, 150)]     // OddsType 1 subtracts 1
    [InlineData(2.0, 2, 100, 50, 200)]     // OddsType 2 uses original odds
    public void GetActualPayout_WithDifferentOdds_CalculatesCorrectly(
        decimal odds, short oddsType, decimal actualStake, decimal stake, decimal expected)
    {
        // Arrange
        const decimal modds = 1.5m; // Greater than ModdsQuarter

        // Act
        var result = _service.GetActualPayout(modds, odds, oddsType, actualStake, stake);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion GetActualPayout Tests

    #region SetValue Tests

    [Fact]
    public void SetValue_WithMaxWinLimit_UpdatesTicketCorrectly()
    {
        // Arrange
        var ticket = CreateTestTicket();
        var prepareTicket = CreatePrepareTicket();
        SetupPrepareTicketService(ticket, prepareTicket, false);

        // Act
        _service.SetValue(ticket, new List<WinLimit>());

        // Assert
        AssertTicketValues(ticket, prepareTicket);
    }

    [Theory]
    [InlineData(true)]   // Recovery case
    [InlineData(false)]  // Non-recovery case
    public void SetValue_WithRecoveryFlag_SetsRefundValues(bool isRecovery)
    {
        // Arrange
        var ticket = CreateTestTicket();
        _prepareTicketServiceMock.Setup(x => x.IsTicketUpdatable(ticket)).Returns(true);

        // Act
        _service.SetValue(ticket, isRecovery);

        // Assert
        AssertRefundValues(ticket);
    }

    #endregion SetValue Tests

    #region RecoverSetValue Tests

    [Theory]
    [InlineData(StatusWinlost.Win, 150)]      // Win case
    [InlineData(StatusWinlost.HalfWin, 125)]  // Half win case
    public void RecoverSetValue_WithWinningStatus_CalculatesCorrectPayout(
        byte statusWinLost, decimal expectedPayout)
    {
        // Arrange
        var ticket = CreateTestTicket();
        ticket.StatusWinLost = statusWinLost;
        var prepareTicket = CreatePrepareTicket();
        prepareTicket.StatusWinLost = statusWinLost;

        SetupPrepareTicketService(ticket, prepareTicket, true);

        // Act
        _service.RecoverSetValue(ticket, new List<WinLimit>());

        // Assert
        Assert.Equal(expectedPayout, ticket.ActualPayout);
        AssertTicketValues(ticket, prepareTicket);
    }

    [Fact]
    public void RecoverSetValue_UpdatesSettlementTime_WhenValuesChange()
    {
        // Arrange
        var ticket = CreateTestTicket();
        var prepareTicket = CreatePrepareTicket();
        prepareTicket.StatusId = StatusId.Lose;
        SetupPrepareTicketService(ticket, prepareTicket, true);

        // Act
        _service.RecoverSetValue(ticket, new List<WinLimit>());

        // Assert
        Assert.True(ticket.IsUpdateSettlementTime);
    }

    #endregion RecoverSetValue Tests

    #region Helper Methods

    private static Ticket CreateTestTicket() => new()
    {
        Odds = 2.0m,
        OddsType = 1,
        ActualStake = 100,
        Stake = 50,
        StatusId = StatusId.Won,
        Status = StatusName.Won,
        WinLost = 150,
        StatusWinLost = StatusWinlost.Win,
        Ruben = Ruben.Default,
        ExtraInfo = "test",
        MaxWinLimit = 1000,
    };

    private static PrepareTicket CreatePrepareTicket() => new()
    {
        IsApplyMaxPayout = false,
        MaxWinLost = 1000,
        StatusId = StatusId.Won,
        Status = StatusName.Won,
        StatusWinLost = StatusWinlost.Win,
        ExtraInfo = "test"
    };

    private void SetupPrepareTicketService(
        Ticket ticket,
        PrepareTicket prepareTicket,
        bool isRecovery)
    {
        var winlost = new Winlost { WinLost = 150, AWinLost = 50, MWinLost = 50, SWinLost = 50 };
        var commission = new Commission { Comm = 10, PlayerComm = 10, AComm = 5, SComm = 5 };

        if (isRecovery)
        {
            _prepareTicketServiceMock.Setup(x => x.RecoverValue(ticket, It.IsAny<List<WinLimit>>()))
                .Returns(prepareTicket);
            _prepareTicketServiceMock.Setup(x => x.RecoverWinLost(ticket, prepareTicket))
                .Returns(winlost);
            _prepareTicketServiceMock.Setup(x => x.RecoverCommission(ticket, prepareTicket))
                .Returns(commission);
        }
        else
        {
            _prepareTicketServiceMock.Setup(x => x.CreateValue(ticket, It.IsAny<List<WinLimit>>()))
                .Returns(prepareTicket);
            _prepareTicketServiceMock.Setup(x => x.CreateWinLost(ticket, prepareTicket))
                .Returns(winlost);
            _prepareTicketServiceMock.Setup(x => x.CreateCommission(ticket, prepareTicket))
                .Returns(commission);
        }
    }

    private static void AssertTicketValues(Ticket ticket, PrepareTicket prepareTicket)
    {
        Assert.Equal(prepareTicket.IsApplyMaxPayout, ticket.IsApplyMaxPayout);
        Assert.Equal(prepareTicket.MaxWinLost, ticket.MaxWinLimit);
        Assert.Equal(prepareTicket.StatusWinLost, ticket.StatusWinLost);
        Assert.Equal(prepareTicket.StatusId, ticket.StatusId);
        Assert.Equal(prepareTicket.Status, ticket.Status);
        Assert.Equal(prepareTicket.ExtraInfo, ticket.ExtraInfo);
        Assert.Equal(Ruben.Default, ticket.Ruben);
    }

    private static void AssertRefundValues(Ticket ticket)
    {
        Assert.Equal(ticket.ActualStake, ticket.WinLost);
        Assert.Equal(0, ticket.AWinLost);
        Assert.Equal(0, ticket.MWinLost);
        Assert.Equal(0, ticket.SWinLost);
        Assert.Equal(0, ticket.PlayerComm);
        Assert.Equal(0, ticket.Comm);
        Assert.Equal(0, ticket.AComm);
        Assert.Equal(0, ticket.SComm);
        Assert.Equal(StatusWinlost.Refund, ticket.StatusWinLost);
        Assert.Equal(StatusId.Refund, ticket.StatusId);
        Assert.Equal(StatusName.Refund, ticket.Status);
        Assert.Equal(Ruben.Default, ticket.Ruben);
    }

    #endregion Helper Methods
}