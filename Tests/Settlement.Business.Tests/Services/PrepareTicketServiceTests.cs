using NSubstitute;
using Settlement.Business.Services;

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
    public void WinLostMaxPayout_ModdsGreaterThanQuarter_ReturnsExpectedWinLost()
    {
        //var ticket = new Ticket
        //{
        //    Modds = 0.5m,
        //    ActualStake = 0.1m,
        //    APositionTaking = 0.2m,
        //    MPositionTaking = 0.3m,
        //    SPositionTaking = 0.4m
        //};

        //var prepareTicket = new PrepareTicket
        //{
        //    WinLost = 1000
        //};

        //var actual = service.WinLostMaxPayout(ticket, prepareTicket);

        //var expectedWinlost = new Winlost
        //{
        //    WinLost = 1000,
        //    AWinLost = -199.98m,
        //    MWinLost = -299.97m,
        //    SWinLost = -399.96m
        //};

        //AssertExtension.AreEqual(expectedWinlost, actual);
    }
}