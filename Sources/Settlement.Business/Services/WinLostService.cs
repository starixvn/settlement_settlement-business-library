using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

using Models;
using Settlement.Business.Core.Constants;

public interface WinLostService
{
    (bool isApplyMaxPayout, decimal? winLost) UpdateWinLost(BetTransaction betTran, List<WinLimit> listWinLimit);
}

public class WinLostServiceImp : WinLostService
{
    public (bool isApplyMaxPayout, decimal? winLost) UpdateWinLost(BetTransaction betTran, List<WinLimit> listWinLimit)
    {
        var winLimit = listWinLimit.Find(w => w.CustId == betTran.CustId);
        if (winLimit != null && CheckMaxWinLimit(betTran, winLimit))
        {
            return (true, winLimit.MaxWinLimit);
        }

        return (false, betTran.WinLost);
    }

    private static bool CheckMaxWinLimit(BetTransaction betTran, WinLimit winLimit)
    {
        return betTran.Modds >= Modds.ModdsQuarter && betTran.ActualPayout > winLimit.MaxWinLimit;
    }
}