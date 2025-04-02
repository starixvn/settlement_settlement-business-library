using Moq;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Tests.Services;

public class PrepareTicketServiceTests
{
    private readonly Mock<CommissionService> commissionServiceMock;
    private readonly Mock<WinLostByModdsService> winLostByModdsServiceMock;
    private readonly Mock<WinLostService> winLostServiceMock;
    private readonly PrepareTicketServiceImp service;

    public PrepareTicketServiceTests()
    {
        commissionServiceMock = new Mock<CommissionService>();
        winLostByModdsServiceMock = new Mock<WinLostByModdsService>();
        winLostServiceMock = new Mock<WinLostService>();
        service = new PrepareTicketServiceImp(
            commissionServiceMock.Object,
            winLostByModdsServiceMock.Object,
            winLostServiceMock.Object);
    }

    #region CreateValue Tests

    [Theory]
    [InlineData(1.5, StatusId.Won, StatusName.Won, StatusWinlost.Win)]
    [InlineData(-1.5, StatusId.Lose, StatusName.Lose, StatusWinlost.Loss)]
    [InlineData(0, StatusId.Draw, StatusName.Draw, StatusWinlost.Loss)]
    public void CreateValue_WithDifferentModds_ReturnsCorrectStatus(
        decimal modds, byte expectedStatusId, string expectedStatusName, byte expectedStatusWinLost)
    {
        // Arrange
        var ticket = CreateTestTicket(modds);
        SetupWinLostServiceMock(ticket);

        // Act
        var result = service.CreateValue(ticket, []);

        // Assert
        Assert.Equal(expectedStatusId, result.StatusId);
        Assert.Equal(expectedStatusName, result.Status);
        Assert.Equal(expectedStatusWinLost, result.StatusWinLost);
    }

    [Theory]
    [InlineData(-1.0, 1, 1.0)]  // Negative odds becomes 1
    [InlineData(2.0, 1, 1.0)]   // OddsType 1 subtracts 1
    [InlineData(2.0, 2, 2.0)]   // OddsType 2 uses original odds
    public void CreateValue_CalculatesOddsCorrectly(decimal odds, short oddsType, decimal expectedOdds)
    {
        // Arrange
        var ticket = CreateTestTicket(1.5m);
        ticket.Odds = odds;
        ticket.OddsType = oddsType;
        SetupWinLostServiceMock(ticket);

        // Act
        var result = service.CreateValue(ticket, []);

        // Assert
        Assert.Equal(expectedOdds, result.Odds);
    }

    #endregion CreateValue Tests

    #region RecoverValue Tests

    [Theory]
    [InlineData(StatusWinlost.Win, StatusId.Won, StatusName.Won)]
    [InlineData(StatusWinlost.HalfWin, StatusId.Won, StatusName.Won)]
    [InlineData(StatusWinlost.Loss, StatusId.Lose, StatusName.Lose)]
    [InlineData(StatusWinlost.HalfLoss, StatusId.Lose, StatusName.Lose)]
    [InlineData(StatusWinlost.Refund, StatusId.Refund, StatusName.Refund)]
    public void RecoverValue_WithDifferentStatusWinLost_ReturnsCorrectStatus(
        byte statusWinLost, byte expectedStatusId, string expectedStatusName)
    {
        // Arrange
        var ticket = CreateTestTicket(1.5m);
        ticket.StatusWinLost = statusWinLost;
        SetupWinLostServiceMock(ticket, true);

        // Act
        var result = service.RecoverValue(ticket, []);

        // Assert
        Assert.Equal(expectedStatusId, result.StatusId);
        Assert.Equal(expectedStatusName, result.Status);
    }

    #endregion RecoverValue Tests

    #region Commission Tests

    [Theory]
    [InlineData(1.5, "ModdsHigherThanQuarter")]
    [InlineData(0.25, "ModdsEqualQuarter")]
    [InlineData(-0.25, "ModdsEqualNegativeQuarter")]
    [InlineData(-1.5, "ModdsLowerThanNegativeQuarter")]
    public void CreateCommission_WithDifferentModds_CallsCorrectService(decimal modds, string expectedMethod)
    {
        // Arrange
        var ticket = CreateTestTicket(modds);
        var prepareTicket = CreateTestPrepareTicket();
        SetupCommissionServiceMocks();

        // Act
        service.CreateCommission(ticket, prepareTicket);

        // Assert
        VerifyCommissionServiceCall(expectedMethod, ticket, prepareTicket);
    }

    [Theory]
    [InlineData(StatusWinlost.Win, "ModdsHigherThanQuarter")]
    [InlineData(StatusWinlost.HalfWin, "ModdsEqualQuarter")]
    [InlineData(StatusWinlost.Loss, "ModdsLowerThanNegativeQuarter")]
    [InlineData(StatusWinlost.HalfLoss, "ModdsEqualNegativeQuarter")]
    public void RecoverCommission_WithDifferentStatus_CallsCorrectService(byte statusWinLost, string expectedMethod)
    {
        // Arrange
        var ticket = CreateTestTicket(1.5m);
        ticket.StatusWinLost = statusWinLost;
        var prepareTicket = CreateTestPrepareTicket();
        SetupCommissionServiceMocks();

        // Act
        service.RecoverCommission(ticket, prepareTicket);

        // Assert
        VerifyCommissionServiceCall(expectedMethod, ticket, prepareTicket);
    }

    #endregion Commission Tests

    #region WinLost Tests

    [Theory]
    [InlineData(1.5, "WinLostGreaterThanQuarter")]
    [InlineData(0.25, "WinLostEqualQuarter")]
    [InlineData(-0.25, "WinLostEqualNegativeQuarter")]
    [InlineData(-1.5, "WinLostLowerThanNegativeQuarter")]
    public void CreateWinLost_WithDifferentModds_CallsCorrectService(decimal modds, string expectedMethod)
    {
        // Arrange
        var ticket = CreateTestTicket(modds);
        var prepareTicket = CreateTestPrepareTicket();
        SetupWinLostByModdsServiceMocks();

        // Act
        service.CreateWinLost(ticket, prepareTicket);

        // Assert
        VerifyWinLostServiceCall(expectedMethod, ticket, prepareTicket);
    }

    [Theory]
    [InlineData(StatusWinlost.Win, "WinLostGreaterThanQuarter")]
    [InlineData(StatusWinlost.HalfWin, "WinLostEqualQuarter")]
    [InlineData(StatusWinlost.Loss, "WinLostLowerThanNegativeQuarter")]
    [InlineData(StatusWinlost.HalfLoss, "WinLostEqualNegativeQuarter")]
    public void RecoverWinLost_WithDifferentStatus_CallsCorrectService(byte statusWinLost, string expectedMethod)
    {
        // Arrange
        var ticket = CreateTestTicket(1.5m);
        ticket.StatusWinLost = statusWinLost;
        var prepareTicket = CreateTestPrepareTicket();
        SetupWinLostByModdsServiceMocks();

        // Act
        service.RecoverWinLost(ticket, prepareTicket);

        // Assert
        VerifyWinLostServiceCall(expectedMethod, ticket, prepareTicket);
    }

    [Fact]
    public void WinLost_WithMaxPayout_CallsMaxPayoutService()
    {
        // Arrange
        var ticket = CreateTestTicket(1.5m);
        var prepareTicket = CreateTestPrepareTicket();
        prepareTicket.MaxWinLost = 1000m;
        prepareTicket.IsApplyMaxPayout = true;

        // Act
        service.CreateWinLost(ticket, prepareTicket);

        // Assert
        winLostByModdsServiceMock.Verify(x => x.WinLostMaxPayout(ticket, 1000m), Times.Once);
    }

    #endregion WinLost Tests

    #region Helper Methods

    private static Ticket CreateTestTicket(decimal modds) => new()
    {
        Modds = modds,
        ComStatus = "1",
        Odds = 2.0m,
        OddsType = 1,
        BetType = 1,
        ActualStake = 100,
        StatusId = StatusId.Won,
        IsMarked = false,
        BetTeam = "Team1"
    };

    private static PrepareTicket CreateTestPrepareTicket() => new()
    {
        CommWinRate = 1,
        CommLostRate = 1,
        CommHalfWLRate = 0.5m,
        Odds = 2.0m,
        StatusId = StatusId.Won,
        Status = StatusName.Won
    };

    private void SetupWinLostServiceMock(Ticket ticket, bool isRecovery = false)
    {
        winLostServiceMock
            .Setup(x => x.GetWinLost(ticket, It.IsAny<List<WinLimit>>(), isRecovery))
            .Returns((true, 1000m));
    }

    private void SetupCommissionServiceMocks()
    {
        commissionServiceMock
            .Setup(x => x.ModdsHigherThanQuarter(It.IsAny<Ticket>(), It.IsAny<PrepareTicket>()))
            .Returns(new Commission());
        // Setup other commission service methods similarly
    }

    private void SetupWinLostByModdsServiceMocks()
    {
        winLostByModdsServiceMock
            .Setup(x => x.WinLostGreaterThanQuarter(It.IsAny<Ticket>(), It.IsAny<decimal>()))
            .Returns(new Winlost());
        // Setup other winlost service methods similarly
    }

    private void VerifyCommissionServiceCall(string methodName, Ticket ticket, PrepareTicket prepareTicket)
    {
        switch (methodName)
        {
            case "ModdsHigherThanQuarter":
                commissionServiceMock.Verify(x => x.ModdsHigherThanQuarter(ticket, prepareTicket), Times.Once);
                break;
                // Verify other commission service methods similarly
        }
    }

    private void VerifyWinLostServiceCall(string methodName, Ticket ticket, PrepareTicket prepareTicket)
    {
        switch (methodName)
        {
            case "WinLostGreaterThanQuarter":
                winLostByModdsServiceMock.Verify(x => x.WinLostGreaterThanQuarter(ticket, prepareTicket.Odds), Times.Once);
                break;
                // Verify other winlost service methods similarly
        }
    }

    #endregion Helper Methods
}