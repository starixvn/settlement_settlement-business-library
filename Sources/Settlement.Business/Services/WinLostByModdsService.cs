using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Services;

public interface WinLostByModdsService
{
    Winlost WinLostMaxPayout(Ticket ticket, decimal maxWinLost);

    Winlost RecoverWinLostMaxPayout(Ticket ticket, decimal maxWinLost);

    Winlost WinLostGreaterThanQuarter(Ticket ticket, decimal odds);

    Winlost WinLostEqualQuarter(Ticket ticket, decimal odds);

    Winlost WinLostEqualNegativeQuarter(Ticket ticket);

    Winlost WinLostLowerThanNegativeQuarter(Ticket ticket);
}

public class WinLostByModdsServiceImp : WinLostByModdsService
{
    private const decimal HalfValue = 0.5m;

    private const int DefaultDeadHeat = 1;

    public Winlost WinLostMaxPayout(Ticket ticket, decimal maxWinLost)
    {
        return ticket.Modds >= Modds.ModdsQuarter
            ? CalculateWinLostWithPositionTaking(ticket, maxWinLost)
            : new Winlost { WinLost = maxWinLost };
    }

    public Winlost RecoverWinLostMaxPayout(Ticket ticket, decimal maxWinLost)
    {
        return ticket.StatusWinLost is StatusWinlost.Win or StatusWinlost.HalfWin
            ? CalculateWinLostWithPositionTaking(ticket, maxWinLost)
            : new Winlost { WinLost = maxWinLost };
    }

    private static Winlost CalculateWinLostWithPositionTaking(Ticket ticket, decimal maxWinLost)
    {
        var winLostDifference = maxWinLost - ticket.ActualStake;
        return new Winlost
        {
            WinLost = maxWinLost,
            AWinLost = winLostDifference * -ticket.APositionTaking,
            MWinLost = winLostDifference * -ticket.MPositionTaking,
            SWinLost = winLostDifference * -ticket.SPositionTaking,
        };
    }

    public Winlost WinLostGreaterThanQuarter(Ticket ticket, decimal odds)
    {
        var deadHeat = ticket.DeadHeat ?? DefaultDeadHeat;
        return new Winlost
        {
            WinLost = CalculateWinLostAmount(ticket, odds),
            AWinLost = CalculatePositionWinLost(ticket.Stake, odds, ticket.APositionTaking, deadHeat),
            MWinLost = CalculatePositionWinLost(ticket.Stake, odds, ticket.MPositionTaking, deadHeat),
            SWinLost = CalculatePositionWinLost(ticket.Stake, odds, ticket.SPositionTaking, deadHeat)
        };
    }

    private static decimal CalculateWinLostAmount(Ticket ticket, decimal odds)
    {
        if ((ticket.DeadHeat ?? 0) > 0)
        {
            return (decimal)((odds / ticket.DeadHeat! + 1) * ticket.Stake)!;
        }

        return ticket.ActualStake + ticket.Stake * odds;
    }

    private static decimal CalculatePositionWinLost(decimal stake, decimal odds, decimal positionTaking, decimal divisor)
    {
        return (stake * odds * -positionTaking) / divisor;
    }

    public Winlost WinLostEqualQuarter(Ticket ticket, decimal odds)
    {
        return new Winlost
        {
            WinLost = CalculateQuarterWinLost(ticket, odds),
            AWinLost = CalculateQuarterWinLost(ticket.Stake, odds, ticket.APositionTaking),
            MWinLost = CalculateQuarterWinLost(ticket.Stake, odds, ticket.MPositionTaking),
            SWinLost = CalculateQuarterWinLost(ticket.Stake, odds, ticket.SPositionTaking),
        };
    }

    private static decimal CalculateQuarterWinLost(Ticket ticket, decimal odds) =>
        ticket.ActualStake + (ticket.Stake * odds * HalfValue);

    private static decimal CalculateQuarterWinLost(decimal stake, decimal odds, decimal positionTaking) =>
        stake * odds * -positionTaking * HalfValue;

    public Winlost WinLostEqualNegativeQuarter(Ticket ticket)
    {
        return new Winlost
        {
            WinLost = ticket.ActualStake * HalfValue,
            AWinLost = CalculateNegativePositionWinLost(ticket.ActualStake, ticket.APositionTaking),
            MWinLost = CalculateNegativePositionWinLost(ticket.ActualStake, ticket.MPositionTaking),
            SWinLost = CalculateNegativePositionWinLost(ticket.ActualStake, ticket.SPositionTaking)
        };
    }

    private static decimal CalculateNegativePositionWinLost(decimal actualStake, decimal positionTaking) =>
        actualStake * positionTaking * HalfValue;

    public Winlost WinLostLowerThanNegativeQuarter(Ticket ticket)
    {
        return new Winlost
        {
            WinLost = 0,
            AWinLost = ticket.ActualStake * ticket.APositionTaking,
            MWinLost = ticket.ActualStake * ticket.MPositionTaking,
            SWinLost = ticket.ActualStake * ticket.SPositionTaking,
        };
    }
}