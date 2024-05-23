using Settlement.Business.Core.Models;

namespace Settlement.Business.Core.Helper;

public static class WinLostHelper
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
}