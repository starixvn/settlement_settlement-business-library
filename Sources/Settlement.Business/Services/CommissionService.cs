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
        => CalculateCommission(ticket, infos.CommWinRate);

    public Commission ModdsEqualQuarter(Ticket ticket, PrepareTicket infos)
        => CalculateCommission(ticket, infos.CommWinRate, infos.CommHalfWLRate);

    public Commission ModdsEqualNegativeQuarter(Ticket ticket, PrepareTicket infos)
        => CalculateCommission(ticket, infos.CommLostRate, infos.CommHalfWLRate);

    public Commission ModdsLowerThanNegativeQuarter(Ticket ticket, PrepareTicket infos)
        => CalculateCommission(ticket, infos.CommLostRate);

    private static Commission CalculateCommission(
        Ticket ticket,
        decimal commissionRate,
        decimal? halfWLRate = null)
    {
        var multiplier = halfWLRate ?? 1m;
        var positionTakings = GetPositionTakings(ticket);

        return new Commission
        {
            Comm = CalculateCommValue(
                commissionRate,
                ticket.Discount,
                ticket.Stake,
                positionTakings.AgentTaking,
                multiplier),

            AComm = CalculateCommValue(
                commissionRate,
                ticket.ADiscount,
                ticket.Stake,
                positionTakings.MasterAndAgentTaking,
                multiplier),

            SComm = CalculateCommValue(
                commissionRate,
                ticket.SDiscount,
                ticket.Stake,
                positionTakings.AllTaking,
                multiplier),

            PlayerComm = commissionRate * ticket.PlayerDiscount * ticket.Stake * multiplier
        };
    }

    private static decimal CalculateCommValue(
        decimal commissionRate,
        decimal discount,
        decimal stake,
        decimal positionTaking,
        decimal multiplier)
        => commissionRate * discount * stake * (1 - positionTaking) * multiplier;

    private static (decimal AgentTaking, decimal MasterAndAgentTaking, decimal AllTaking) GetPositionTakings(Ticket ticket)
        => (
            ticket.APositionTaking,
            ticket.MPositionTaking + ticket.APositionTaking,
            ticket.SPositionTaking + ticket.MPositionTaking + ticket.APositionTaking
        );
}