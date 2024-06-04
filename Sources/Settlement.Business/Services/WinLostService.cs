using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

using Models;
using Settlement.Business.Core.Constants;

public interface WinLostService
{
    (bool isApplyMaxPayout, decimal? winLost) GetWinLost(BetTran betTran, List<WinLimit> listWinLimit);
}

public class WinLostServiceImp : WinLostService
{
    public (bool isApplyMaxPayout, decimal? winLost) GetWinLost(BetTran betTran, List<WinLimit> listWinLimit)
    {
        var winLimit = listWinLimit.Find(w => w.CustId == betTran.CustId);
        var actualPo = GetActualPayout(betTran);
        if (winLimit != null && actualPo > winLimit.MaxWinLimit)
        {
            betTran.ActualPayout = (decimal)actualPo;
            return (true, winLimit.MaxWinLimit);
        }

        return (false, betTran.WinLost);
    }


    private static decimal? GetActualPayout(BetTran betTran)
    {
        var odds = betTran.Odds < 0 ? 1 : betTran.OddsType == 1 ? betTran.Odds - 1 : betTran.Odds;
        return betTran.Modds switch
        {
            > Modds.ModdsQuarter => betTran.ActualStake + betTran.Stake * odds,
            Modds.ModdsQuarter => betTran.ActualStake + (betTran.Stake * 0.5m * odds),
            _ => null
        };
    }
}