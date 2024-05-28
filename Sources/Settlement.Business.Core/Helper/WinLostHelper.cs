using Settlement.Business.Core.Models;

namespace Settlement.Business.Core.Helper;

public static class WinLostHelper
{
    public static decimal AWinLost(BetTran betTran, decimal odds)
    {
        if (betTran is { WinLost: not null, IsApplyMaxPayout: true })
        {
            return (decimal)(betTran.Modds > 0 ? (betTran.WinLost - betTran.ActualStake) * betTran.APositionTaking : 0);
        }

        var deadHeat = betTran.DeadHeat == 0 ? 1 : betTran.DeadHeat;
        return betTran.Modds > 0
            ? betTran.Stake * odds * -betTran.APositionTaking / deadHeat
            : betTran.ActualStake * betTran.APositionTaking;
    }

    public static decimal MWinLost(BetTran betTran, decimal odds)
    {
        if (betTran is { WinLost: not null, IsApplyMaxPayout: true })
        {
            return (decimal)(betTran.Modds > 0 ? (betTran.WinLost - betTran.ActualStake) * betTran.MPositionTaking : 0);
        }

        var deadHeat = betTran.DeadHeat == 0 ? 1 : betTran.DeadHeat;
        return betTran.Modds > 0
            ? betTran.Stake * odds * -betTran.MPositionTaking / deadHeat
            : betTran.ActualStake * betTran.MPositionTaking;
    }

    public static decimal SWinLost(BetTran betTran, decimal odds)
    {
        if (betTran is { WinLost: not null, IsApplyMaxPayout: true })
        {
            return (decimal)(betTran.Modds > 0 ? (betTran.WinLost - betTran.ActualStake) * betTran.SPositionTaking : 0);
        }

        var deadHeat = betTran.DeadHeat == 0 ? 1 : betTran.DeadHeat;
        return betTran.Modds > 0
            ? betTran.Stake * odds * -betTran.SPositionTaking / deadHeat
            : betTran.ActualStake * betTran.SPositionTaking;
    }
}