using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface WinLostByModdsService
{
    Winlost WinLostMaxPayout(Ticket ticket, decimal maxWinLost);

    Winlost WinLostGreaterThanQuarter(Ticket ticket, decimal odds);

    Winlost WinLostEqualQuarter(Ticket ticket, decimal odds);

    Winlost WinLostEqualNegativeQuarter(Ticket ticket);

    Winlost WinLostLowerThanNegativeQuarter(Ticket ticket);
}

public class WinLostByModdsServiceImp : WinLostByModdsService
{
    private const decimal halfValue = 0.5m;

    public Winlost WinLostMaxPayout(Ticket ticket, decimal maxWinLost)
    {
        if (ticket.Modds >= Modds.ModdsQuarter)
        {
            return new Winlost
            {
                WinLost = maxWinLost,
                AWinLost = (maxWinLost - ticket.ActualStake) * -ticket.APositionTaking,
                MWinLost = (maxWinLost - ticket.ActualStake) * -ticket.MPositionTaking,
                SWinLost = (maxWinLost - ticket.ActualStake) * -ticket.SPositionTaking,
            };
        }

        return new Winlost { WinLost = maxWinLost };
    }

    public Winlost WinLostGreaterThanQuarter(Ticket ticket, decimal odds)
    {
        var deadHeat = ticket.DeadHeat ?? 1;
        return new Winlost
        {
            WinLost = (decimal)((ticket.DeadHeat ?? 0) > 0 ? ((odds / ticket.DeadHeat! + 1) * ticket.Stake)! : (ticket.ActualStake + ticket.Stake * odds)),
            AWinLost = ticket.Stake * odds * -ticket.APositionTaking / deadHeat,
            MWinLost = ticket.Stake * odds * -ticket.MPositionTaking / deadHeat,
            SWinLost = ticket.Stake * odds * -ticket.SPositionTaking / deadHeat,
        };
    }

    public Winlost WinLostEqualQuarter(Ticket ticket, decimal odds)
    {
        return new Winlost
        {
            WinLost = ticket.ActualStake + ticket.Stake * odds * halfValue,
            AWinLost = ticket.Stake * odds * -ticket.APositionTaking * halfValue,
            MWinLost = ticket.Stake * odds * -ticket.MPositionTaking * halfValue,
            SWinLost = ticket.Stake * odds * -ticket.SPositionTaking * halfValue,
        };
    }

    public Winlost WinLostEqualNegativeQuarter(Ticket ticket)
    {
        return new Winlost
        {
            WinLost = ticket.ActualStake * halfValue,
            AWinLost = ticket.ActualStake * ticket.APositionTaking * halfValue,
            MWinLost = ticket.ActualStake * ticket.MPositionTaking * halfValue,
            SWinLost = ticket.ActualStake * ticket.SPositionTaking * halfValue,
        };
    }

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