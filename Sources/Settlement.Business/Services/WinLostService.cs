using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface WinLostService
{
    (bool isApplyMaxPayout, decimal? winLost) GetWinLost(Ticket ticket, List<WinLimit> listWinLimit);
}

public class WinLostServiceImp : WinLostService
{
    public (bool isApplyMaxPayout, decimal? winLost) GetWinLost(Ticket ticket, List<WinLimit> listWinLimit)
    {
        var winLimit = listWinLimit.Find(w => w.CustId == ticket.CustId);
        if (winLimit != null && CheckMaxWinLimit(ticket.Modds, ticket.ActualPayout, winLimit.MaxWinLimit))
        {
            return (true, winLimit.MaxWinLimit);
        }

        return (false, ticket.WinLost);
    }

    private static bool CheckMaxWinLimit(decimal modds, decimal? actualPayout, decimal maxWinLimit)
    {
        return modds >= Modds.ModdsQuarter && actualPayout > maxWinLimit;
    }
}