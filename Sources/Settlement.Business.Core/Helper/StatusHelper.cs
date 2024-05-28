using Settlement.Business.Core.Models;

namespace Settlement.Business.Core.Helper;

public static class StatusHelper
{
    public static DateTime GetSettlementTime(BetTran betTrans, decimal? winLost, byte statusId)
    {
        return betTrans.WinLost != winLost
               || betTrans.StatusId != statusId ? DateTime.Now : betTrans.SettlementTime;
    }

    public static string? GetExtraInfo(BetTran betTrans)
    {
        return betTrans.IsApplyMaxPayout
            ? betTrans.ExtraInfo ?? $"<l255 apo=\"{betTrans.ActualPayout}\" />"
            : betTrans.ExtraInfo;
    }

    public static byte GetStatusWinLost(decimal mOdds)
    {
        const byte win = 2;

        const byte lost = 4;

        return mOdds > 0 ? win : lost;
    }
}