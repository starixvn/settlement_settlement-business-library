using Settlement.Business.Core.Models;
using static Settlement.Business.Core.Constants.Status;

namespace HappyFiveBusiness.Services;

public static class CalculateFunction
{
    public static decimal WinLost(Transaction tx, BetTransaction betTrans, decimal odds)
    {
        if (tx is { WinLost: not null, IsMaxPayout: true })
        {
            return (decimal)tx.WinLost;
        }

        if (tx.MOdds > 0)
        {
            return tx.DeadHeat > 0 ?
                (odds / tx.DeadHeat + 1) * betTrans.Stake
                : betTrans.ActualStake + betTrans.Stake * tx.MOdds;

        }

        return 0;
    }

    public static decimal AWinLost(Transaction tx, BetTransaction betTrans, decimal odds)
    {
        if (tx.WinLost is not null && tx.IsMaxPayout)
        {
            return (decimal)(tx.MOdds > 0 ? (tx.WinLost - betTrans.ActualStake) * betTrans.APositionTaking : 0);
        }

        var deadHeat = tx.DeadHeat == 0 ? 1 : tx.DeadHeat;
        return tx.MOdds > 0
            ? betTrans.Stake * odds * -betTrans.APositionTaking / deadHeat
            : betTrans.ActualStake * betTrans.APositionTaking;
    }

    public static decimal MWinLost(Transaction tx, BetTransaction betTrans, decimal odds)
    {
        if (tx.WinLost is not null && tx.IsMaxPayout)
        {
            return (decimal)(tx.MOdds > 0 ? (tx.WinLost - betTrans.ActualStake) * betTrans.MPositionTaking : 0);
        }

        var deadHeat = tx.DeadHeat == 0 ? 1 : tx.DeadHeat;
        return tx.MOdds > 0
            ? betTrans.Stake * odds * -betTrans.MPositionTaking / deadHeat
            : betTrans.ActualStake * betTrans.MPositionTaking;
    }

    public static decimal SWinLost(Transaction tx, BetTransaction betTrans, decimal odds)
    {
        if (tx.WinLost is not null && tx.IsMaxPayout)
        {
            return (decimal)(tx.MOdds > 0 ? (tx.WinLost - betTrans.ActualStake) * betTrans.SPositionTaking : 0);
        }

        var deadHeat = tx.DeadHeat == 0 ? 1 : tx.DeadHeat;
        return tx.MOdds > 0
            ? betTrans.Stake * odds * -betTrans.SPositionTaking / deadHeat
            : betTrans.ActualStake * betTrans.SPositionTaking;
    }

    public static decimal PlayerComm(BetTransaction betTrans, decimal mOdds, PrepareBetTranInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.PlayerDiscount * betTrans.Stake
            : infos.CommLostRate * betTrans.PlayerDiscount * betTrans.Stake;
    }

    public static decimal Comm(BetTransaction betTrans, decimal mOdds, PrepareBetTranInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking)
            : infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking);
    }

    public static decimal AComm(BetTransaction betTrans, decimal mOdds, PrepareBetTranInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking))
            : infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking));
    }

    public static decimal SComm(BetTransaction betTrans, decimal mOdds, PrepareBetTranInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking))
            : infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking));
    }

    public static byte CommWinRate(string comStatus)
    {
        return (byte)(comStatus == "1" ? 0 : 1);
    }

    public static byte CommLostRate(string comStatus)
    {
        return (byte)(comStatus == "1" ? 2 : 1);
    }

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

    public static byte GetStatusWL(decimal mOdds)
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