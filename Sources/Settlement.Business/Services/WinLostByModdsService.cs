using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface WinLostByModdsService
{
    Winlost WinLostMaxPayout(Ticket ticket, PrepareTicket infos);

    Winlost WinLostGreaterThanQuarter(Ticket ticket, PrepareTicket infos);

    Winlost WinLostEqualQuarter(Ticket ticket, PrepareTicket infos);

    Winlost WinLostEqualNegativeQuarter(Ticket ticket);

    Winlost WinLostLowerThanNegativeQuarter(Ticket ticket);
}

public class WinLostByModdsServiceImp : WinLostByModdsService
{
    private const decimal halfValue = 0.5m;

    public Winlost WinLostMaxPayout(Ticket ticket, PrepareTicket infos)
    {
        var winlost = (decimal)infos.WinLost!;

        if (ticket.Modds >= Modds.ModdsQuarter)
        {
            return new Winlost
            {
                WinLost = winlost,
                AWinLost = (winlost - ticket.ActualStake) * -ticket.APositionTaking,
                MWinLost = (winlost - ticket.ActualStake) * -ticket.MPositionTaking,
                SWinLost = (winlost - ticket.ActualStake) * -ticket.SPositionTaking,
            };
        }

        return new Winlost() { WinLost = winlost };
    }

    public Winlost WinLostGreaterThanQuarter(Ticket ticket, PrepareTicket infos)
    {
        var deadHeat = ticket.DeadHeat ?? 1;
        return new Winlost
        {
            WinLost = (decimal)((ticket.DeadHeat ?? 0) > 0 ? ((infos.Odds / ticket.DeadHeat! + 1) * ticket.Stake)! : (ticket.ActualStake + ticket.Stake * infos.Odds)),
            AWinLost = ticket.Stake * infos.Odds * -ticket.APositionTaking / deadHeat,
            MWinLost = ticket.Stake * infos.Odds * -ticket.MPositionTaking / deadHeat,
            SWinLost = ticket.Stake * infos.Odds * -ticket.SPositionTaking / deadHeat,
        };
    }

    public Winlost WinLostEqualQuarter(Ticket ticket, PrepareTicket infos)
    {
        return new Winlost
        {
            WinLost = ticket.ActualStake + ticket.Stake * infos.Odds * halfValue,
            AWinLost = ticket.Stake * infos.Odds * -ticket.APositionTaking * halfValue,
            MWinLost = ticket.Stake * infos.Odds * -ticket.MPositionTaking * halfValue,
            SWinLost = ticket.Stake * infos.Odds * -ticket.SPositionTaking * halfValue,
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