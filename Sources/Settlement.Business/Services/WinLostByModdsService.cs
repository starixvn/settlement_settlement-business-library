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
            WinLost = ticket.ActualStake + ticket.Stake * infos.Odds * 0.5m,
            AWinLost = ticket.Stake * infos.Odds * -ticket.APositionTaking * 0.5m,
            MWinLost = ticket.Stake * infos.Odds * -ticket.MPositionTaking * 0.5m,
            SWinLost = ticket.Stake * infos.Odds * -ticket.SPositionTaking * 0.5m,
        };
    }

    public Winlost WinLostEqualNegativeQuarter(Ticket ticket)
    {
        return new Winlost
        {
            WinLost = ticket.ActualStake * 0.5m,
            AWinLost = ticket.ActualStake * ticket.APositionTaking * 0.5m,
            MWinLost = ticket.ActualStake * ticket.MPositionTaking * 0.5m,
            SWinLost = ticket.ActualStake * ticket.SPositionTaking * 0.5m,
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