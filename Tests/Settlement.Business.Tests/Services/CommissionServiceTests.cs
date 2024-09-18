using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class CommissionServiceTests
{
    private readonly CommissionService service = new CommissionServiceImp();

    [Fact]
    public void ModdsHigherThanQuarter_Always_ReturnsExpectedCommission()
    {
        var ticket = new Ticket
        {
            Discount = 0.5m,
            ADiscount = 1.5m,
            SDiscount = 2.5m,
            Stake = 2,
            APositionTaking = 0.5m,
            MPositionTaking = 0.5m,
            SPositionTaking = 0.5m,
            PlayerDiscount = 1
        };
        var prepareTicket = new PrepareTicket
        {
            CommWinRate = 0.1m
        };

        var actual = service.ModdsHigherThanQuarter(ticket, prepareTicket);

        var expectedCommission = new Commission
        {
            Comm = 0.050m,
            AComm = 0.000m,
            SComm = -0.250m,
            PlayerComm = 0.2m
        };

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void ModdsEqualQuarter_Always_ReturnsExpectedCommission()
    {
        var ticket = new Ticket
        {
            Discount = 0.5m,
            ADiscount = 1.5m,
            SDiscount = 2.5m,
            Stake = 2,
            APositionTaking = 1.5m,
            MPositionTaking = 2.5m,
            SPositionTaking = 3.5m,
            PlayerDiscount = 1
        };
        var prepareTicket = new PrepareTicket
        {
            CommWinRate = 0.5m,
            CommHalfWLRate = 1m
        };

        var actual = service.ModdsEqualQuarter(ticket, prepareTicket);

        var expectedCommission = new Commission
        {
            Comm = -0.250m,
            AComm = -4.500m,
            SComm = -16.250m,
            PlayerComm = 1.0m
        };

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void ModdsEqualNegativeQuarter_Always_ReturnsExpectedCommission()
    {
        var ticket = new Ticket
        {
            Discount = 0.5m,
            ADiscount = 1.5m,
            SDiscount = 2.5m,
            Stake = 2,
            APositionTaking = 1.5m,
            MPositionTaking = 2.5m,
            SPositionTaking = 3.5m,
            PlayerDiscount = 1
        };
        var prepareTicket = new PrepareTicket
        {
            CommLostRate = 0.25m,
            CommHalfWLRate = 0.5m
        };

        var actual = service.ModdsEqualNegativeQuarter(ticket, prepareTicket);

        var expectedCommission = new Commission
        {
            Comm = -0.06250m,
            AComm = -1.12500m,
            SComm = -4.06250m,
            PlayerComm = 0.250m
        };

        AssertExtension.AreEqual(expectedCommission, actual);
    }

    [Fact]
    public void ModdsLowerThanNegativeQuarter_Always_ReturnsExpectedCommission()
    {
        var ticket = new Ticket
        {
            Discount = 0.5m,
            ADiscount = 1.5m,
            SDiscount = 2.5m,
            Stake = 2,
            APositionTaking = 1.5m,
            MPositionTaking = 2.5m,
            SPositionTaking = 3.5m,
            PlayerDiscount = 1
        };
        var prepareTicket = new PrepareTicket
        {
            CommLostRate = 2m,
        };

        var actual = service.ModdsLowerThanNegativeQuarter(ticket, prepareTicket);

        var expectedCommission = new Commission
        {
            Comm = -1.00m,
            AComm = -18.00m,
            SComm = -65.00m,
            PlayerComm = 4m
        };

        AssertExtension.AreEqual(expectedCommission, actual);
    }
}