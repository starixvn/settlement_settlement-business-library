using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Core.Helper;

public static class WinLostHelper
{
    public static Winlost CreateWinLost(BetTran betTrans, PrepareBetTran infos)
    {
        if (infos is { WinLost: not null, IsApplyMaxPayout: true })
        {
            return WinLostMaxPayout(betTrans, infos);
        }

        return betTrans.Modds switch
        {
            < Modds.NegativeModdsQuarter => WinLostLowerThanNegativeQuarter(betTrans),
            Modds.NegativeModdsQuarter => WinLostEqualNegativeQuarter(betTrans),
            Modds.ModdsQuarter => WinLostEqualQuarter(betTrans, infos),
            > Modds.ModdsQuarter => WinLostGreaterThanQuarter(betTrans, infos),
            _ => new Winlost { WinLost = betTrans.ActualStake }
        };

    }

    private static Winlost WinLostMaxPayout(BetTran betTran, PrepareBetTran infos)
    {
        var winlost = (decimal)infos.WinLost!;

        if (betTran.Modds >= Modds.ModdsQuarter)
        {
            return new Winlost
            {
                WinLost = winlost,
                AWinLost = (winlost - betTran.ActualStake) * -betTran.APositionTaking,
                MWinLost = (winlost - betTran.ActualStake) * -betTran.MPositionTaking,
                SWinLost = (winlost - betTran.ActualStake) * -betTran.SPositionTaking,
            };
        }

        return new Winlost() { WinLost = winlost };
    }
    private static Winlost WinLostGreaterThanQuarter(BetTran betTran, PrepareBetTran infos)
    {
        var deadHeat = betTran.DeadHeat ?? 1;
        return new Winlost
        {
            WinLost = (decimal)((betTran.DeadHeat ?? 0) > 0 ? ((infos.Odds / betTran.DeadHeat! + 1) * betTran.Stake)! : (betTran.ActualStake + betTran.Stake * infos.Odds)),
            AWinLost = betTran.Stake * infos.Odds * -betTran.APositionTaking / deadHeat,
            MWinLost = betTran.Stake * infos.Odds * -betTran.MPositionTaking / deadHeat,
            SWinLost = betTran.Stake * infos.Odds * -betTran.SPositionTaking / deadHeat,
        };
    }

    private static Winlost WinLostEqualQuarter(BetTran betTran, PrepareBetTran infos)
    {
        return new Winlost
        {
            WinLost = betTran.ActualStake + betTran.Stake * infos.Odds * 0.5m,
            AWinLost = betTran.Stake * infos.Odds * -betTran.APositionTaking * 0.5m,
            MWinLost = betTran.Stake * infos.Odds * -betTran.MPositionTaking * 0.5m,
            SWinLost = betTran.Stake * infos.Odds * -betTran.SPositionTaking * 0.5m,
        };
    }

    private static Winlost WinLostEqualNegativeQuarter(BetTran betTran)
    {
        return new Winlost
        {
            WinLost = betTran.ActualStake * 0.5m,
            AWinLost = betTran.ActualStake * betTran.APositionTaking * 0.5m,
            MWinLost = betTran.ActualStake * betTran.MPositionTaking * 0.5m,
            SWinLost = betTran.ActualStake * betTran.SPositionTaking * 0.5m,
        };
    }

    private static Winlost WinLostLowerThanNegativeQuarter(BetTran betTran)
    {
        return new Winlost
        {
            WinLost = 0,
            AWinLost = betTran.ActualStake * betTran.APositionTaking,
            MWinLost = betTran.ActualStake * betTran.MPositionTaking,
            SWinLost = betTran.ActualStake * betTran.SPositionTaking,
        };
    }

}