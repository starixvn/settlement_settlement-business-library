using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface CommissionService
{
    Commission ModdsHigherThanQuarter(Ticket ticket, PrepareTicket infos);

    Commission ModdsEqualQuarter(Ticket ticket, PrepareTicket infos);

    Commission ModdsEqualNegativeQuarter(Ticket ticket, PrepareTicket infos);

    Commission ModdsLowerThanNegativeQuarter(Ticket ticket, PrepareTicket infos);
}

public class CommissionServiceImp : CommissionService
{
    public Commission ModdsHigherThanQuarter(Ticket ticket, PrepareTicket infos)
    {
        return new Commission
        {
            Comm = infos.CommWinRate * ticket.Discount * ticket.Stake * (1 - ticket.APositionTaking),
            AComm = infos.CommWinRate * ticket.ADiscount * ticket.Stake * (1 - (ticket.MPositionTaking + ticket.APositionTaking)),
            SComm = infos.CommWinRate * ticket.SDiscount * ticket.Stake * (1 - (ticket.SPositionTaking + ticket.MPositionTaking + ticket.APositionTaking)),
            PlayerComm = infos.CommWinRate * ticket.PlayerDiscount * ticket.Stake
        };
    }

    public Commission ModdsEqualQuarter(Ticket ticket, PrepareTicket infos)
    {
        return new Commission
        {
            Comm = infos.CommWinRate * ticket.Discount * ticket.Stake * (1 - ticket.APositionTaking) * infos.CommHalfWLRate,
            AComm = infos.CommWinRate * ticket.ADiscount * ticket.Stake * (1 - (ticket.MPositionTaking + ticket.APositionTaking)) * infos.CommHalfWLRate,
            SComm = infos.CommWinRate * ticket.SDiscount * ticket.Stake * (1 - (ticket.SPositionTaking + ticket.MPositionTaking + ticket.APositionTaking)) * infos.CommHalfWLRate,
            PlayerComm = infos.CommWinRate * ticket.PlayerDiscount * ticket.Stake * infos.CommHalfWLRate
        };
    }

    public Commission ModdsEqualNegativeQuarter(Ticket ticket, PrepareTicket infos)
    {
        return new Commission
        {
            Comm = infos.CommLostRate * ticket.Discount * ticket.Stake * (1 - ticket.APositionTaking) * infos.CommHalfWLRate,
            AComm = infos.CommLostRate * ticket.ADiscount * ticket.Stake * (1 - (ticket.MPositionTaking + ticket.APositionTaking)) * infos.CommHalfWLRate,
            SComm = infos.CommLostRate * ticket.SDiscount * ticket.Stake * (1 - (ticket.SPositionTaking + ticket.MPositionTaking + ticket.APositionTaking)) * infos.CommHalfWLRate,
            PlayerComm = infos.CommLostRate * ticket.PlayerDiscount * ticket.Stake * infos.CommHalfWLRate
        };
    }

    public Commission ModdsLowerThanNegativeQuarter(Ticket ticket, PrepareTicket infos)
    {
        return new Commission
        {
            Comm = infos.CommLostRate * ticket.Discount * ticket.Stake * (1 - ticket.APositionTaking),
            AComm = infos.CommLostRate * ticket.ADiscount * ticket.Stake * (1 - (ticket.MPositionTaking + ticket.APositionTaking)),
            SComm = infos.CommLostRate * ticket.SDiscount * ticket.Stake * (1 - (ticket.SPositionTaking + ticket.MPositionTaking + ticket.APositionTaking)),
            PlayerComm = infos.CommLostRate * ticket.PlayerDiscount * ticket.Stake
        };
    }
}