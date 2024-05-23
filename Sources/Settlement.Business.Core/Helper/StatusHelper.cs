using Settlement.Business.Core.Models;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Core.Helper;

public class StatusHelper
{
    public static int GetStatusId(byte mOdds)
    {
        return mOdds > 0 ? StatusId.Won : StatusId.Lose;
    }

    public static bool IsUpdVoid(BetTransaction betTrans, Transaction tx)
    {
        return betTrans.StatusId != StatusId.Void && !tx.IsMarked;
    }

    public static byte GetStatusId(decimal mOdds)
    {
        return mOdds > 0 ? StatusId.Won : StatusId.Lose;
    }

    public static string GetStatus(decimal mOdds)
    {
        return mOdds > 0 ? StatusName.Won : StatusName.Lose;
    }

    public static DateTime GetSettlementTime(BetTransaction betTrans, decimal winLost, byte statusId)
    {
        return betTrans.WinLost != winLost
               || betTrans.StatusId != statusId ? DateTime.Now : betTrans.SettlementTime;
    }

    public static string GetExtraInfo(BetTransaction betTrans, Transaction tx)
    {
        return tx.IsMaxPayout
            ? betTrans.ExtraInfo ?? string.Empty + $"<l255 apo=\"{tx.ActualPO}\" />"
            : betTrans.ExtraInfo;
    }

    public static byte GetStatusWinLost(decimal mOdds)
    {
        const byte win = 2;

        const byte lost = 4;

        return mOdds > 0 ? win : lost;
    }

    public static decimal GetOdds(BetTransaction betTrans, Transaction tx)
    {
        if (betTrans.Odds < 0)
        {
            return 1;
        }

        if (tx.OddsType == 1)
        {
            return betTrans.Odds - 1;
        }

        return betTrans.Odds;
    }
}