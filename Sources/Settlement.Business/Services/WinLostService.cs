using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface WinLostService
{
    (bool isApplyMaxPayout, decimal? winLost) GetWinLost(Ticket ticket, List<WinLimit> listWinLimit, bool isRecovery);
}

public class WinLostServiceImp : WinLostService
{
    public (bool isApplyMaxPayout, decimal? winLost) GetWinLost(Ticket ticket, List<WinLimit> listWinLimit, bool isRecovery)
    {
        var winLimit = listWinLimit.Find(w => w.CustId == ticket.CustId);
        if (winLimit is null)
        {
            return (false, ticket.WinLost);
        }

        var shouldApplyMaxLimit = ShouldApplyMaxWinLimit(ticket, winLimit.MaxWinLimit, isRecovery);
        return shouldApplyMaxLimit
            ? (true, winLimit.MaxWinLimit)
            : (false, ticket.WinLost);
    }

    private static bool ShouldApplyMaxWinLimit(Ticket ticket, decimal maxWinLimit, bool isRecovery)
    {
        if (ticket.ActualPayout <= maxWinLimit)
        {
            return false;
        }

        return isRecovery
            ? IsWinningStatus(ticket.StatusWinLost)
            : IsWinningOdds(ticket.Modds);
    }

    private static bool IsWinningOdds(decimal modds) => modds >= Modds.ModdsQuarter;

    private static bool IsWinningStatus(byte? statusWinLost) => statusWinLost is Status.StatusWinlost.Win or Status.StatusWinlost.HalfWin;
}