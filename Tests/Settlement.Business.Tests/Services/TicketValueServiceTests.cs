using NSubstitute;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class TicketValueServiceTests
{
    private readonly PrepareTicketService prepareTicketService;

    private readonly TicketValueServiceImp service;

    public TicketValueServiceTests()
    {
        prepareTicketService = Substitute.For<PrepareTicketService>();
        service = new TicketValueServiceImp(prepareTicketService);
    }

    [Fact]
    public void GetActualPayout_InValidModds_ReturnsNull()
    {
        const decimal actualStake = 100;
        const decimal stake = 100;

        var actual = service.GetActualPayout(-1, 1, 2, actualStake, stake);

        Assert.Null(actual);
    }

    [Theory]
    [InlineData(1, -1, 0, 200)]
    [InlineData(1, 1, 1, 100)]
    [InlineData(1, 1, 2, 200)]
    [InlineData(0.25, 1, 2, 150)]
    public void GetActualPayout_Always_ReturnsExpectedValue(decimal modds, decimal odds, short oddsType, decimal expectedValue)
    {
        const decimal actualStake = 100;
        const decimal stake = 100;

        var actual = service.GetActualPayout(modds, odds, oddsType, actualStake, stake);

        Assert.Equal(expectedValue, actual);
    }

    [Fact]
    public void SetValue_IsUpdatedEqualFalse_ReturnsNotCallCreateWinLostAndCreateCommissionMethod()
    {
        var prepareTicket = new PrepareTicket
        {
            IsUpdated = false
        };

        var ticket = new Ticket
        {
            CustId = 1,
            Modds = 0.5m,
            ActualPayout = 6000,
            AWinLost = 0,
            MWinLost = 0,
            SWinLost = 0,
            WinLost = 0,
            PlayerComm = 0,
            Comm = 0,
            AComm = 0,
            SComm = 0
        };

        var listWinLimit = new List<WinLimit>();

        prepareTicketService.CreateValue(Arg.Any<Ticket>(), Arg.Any<List<WinLimit>>()).Returns(prepareTicket);

        service.SetValue(ticket, listWinLimit);

        prepareTicketService.DidNotReceive().CreateWinLost(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>());
        prepareTicketService.DidNotReceive().CreateCommission(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>());

        Assert.False(ticket.IsUpdateSettlementTime);
        Assert.True(ticket is { AWinLost: 0, MWinLost: 0, SWinLost: 0, WinLost: 0 });
        Assert.True(ticket is { PlayerComm: 0, Comm: 0, AComm: 0, SComm: 0 });
    }

    [Fact]
    public void SetValue_IsUpdatedEqualTrue_ReturnsResetNewTicketValue()
    {
        var prepareTicket = new PrepareTicket
        {
            IsUpdated = true
        };

        var ticket = new Ticket
        {
            CustId = 1,
            Modds = 0.5m,
            ActualPayout = 6000,
            AWinLost = 0,
            MWinLost = 0,
            SWinLost = 0,
            WinLost = 0,
            PlayerComm = 0,
            Comm = 0,
            AComm = 0,
            SComm = 0
        };

        var winlost = new Winlost
        {
            AWinLost = -100,
            MWinLost = -200,
            SWinLost = -300,
            WinLost = 400,
        };

        var commission = new Commission
        {
            PlayerComm = 1,
            Comm = 2,
            AComm = 3,
            SComm = 4
        };

        var listWinLimit = new List<WinLimit>();

        prepareTicketService.CreateValue(Arg.Any<Ticket>(), Arg.Any<List<WinLimit>>()).Returns(prepareTicket);

        prepareTicketService.CreateWinLost(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(winlost);
        prepareTicketService.CreateCommission(Arg.Any<Ticket>(), Arg.Any<PrepareTicket>()).Returns(commission);

        service.SetValue(ticket, listWinLimit);

        Assert.Equal(1, Convert.ToByte(ticket.Ruben));
        Assert.True(ticket.IsUpdateSettlementTime);
        Assert.True(ticket is { AWinLost: -100, MWinLost: -200, SWinLost: -300, WinLost: 400 });
        Assert.True(ticket is { PlayerComm: 1, Comm: 2, AComm: 3, SComm: 4 });
    }
}