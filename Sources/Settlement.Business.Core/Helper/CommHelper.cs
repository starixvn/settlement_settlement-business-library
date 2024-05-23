using Settlement.Business.Core.Models;

namespace Settlement.Business.Core.Helper;

public static class CommHelper
{
    public static decimal PlayerComm(BetTransaction betTrans, decimal mOdds, PrepareBetTransInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.PlayerDiscount * betTrans.Stake
            : infos.CommLostRate * betTrans.PlayerDiscount * betTrans.Stake;
    }

    public static decimal Comm(BetTransaction betTrans, decimal mOdds, PrepareBetTransInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking)
            : infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking);
    }

    public static decimal AComm(BetTransaction betTrans, decimal mOdds, PrepareBetTransInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking))
            : infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking));
    }

    public static decimal SComm(BetTransaction betTrans, decimal mOdds, PrepareBetTransInfo infos)
    {
        return mOdds > 0
            ? infos.CommWinRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking))
            : infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking));
    }
}