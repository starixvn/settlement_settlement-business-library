using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Core.Helper;

public static class CommHelper
{
    public static Commission CreateCommission(BetTran betTrans, decimal mOdds, PrepareBetTran infos)
    {
        return mOdds switch
        {
            < Modds.NegativeModdsQuarter => ModdsLowerThanNegativeQuarter(betTrans, infos),
            Modds.NegativeModdsQuarter => ModdsEqualNegativeQuarter(betTrans, infos),
            Modds.ModdsQuarter => ModdsEqualQuarter(betTrans, infos),
            > Modds.ModdsQuarter => ModdsHigherThanQuarter(betTrans, infos),
            _ => new Commission()
        };
    }

    private static Commission ModdsHigherThanQuarter(BetTran betTrans, PrepareBetTran infos)
    {
        return new Commission()
        {
            Comm = infos.CommWinRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking),
            AComm = infos.CommWinRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking)),
            SComm = infos.CommWinRate * betTrans.SDiscount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking)),
            PlayerComm = infos.CommWinRate * betTrans.PlayerDiscount * betTrans.Stake
        };
    }
    private static Commission ModdsEqualQuarter(BetTran betTrans, PrepareBetTran infos)
    {
        return new Commission()
        {
            Comm = infos.CommWinRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking) * infos.CommHalfWLRate,
            AComm = infos.CommWinRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking)) * infos.CommHalfWLRate,
            SComm = infos.CommWinRate * betTrans.SDiscount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking)) * infos.CommHalfWLRate,
            PlayerComm = infos.CommWinRate * betTrans.PlayerDiscount * betTrans.Stake * infos.CommHalfWLRate
        };
    }

    private static Commission ModdsEqualNegativeQuarter(BetTran betTrans, PrepareBetTran infos)
    {
        return new Commission()
        {
            Comm = infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking) * infos.CommHalfWLRate,
            AComm = infos.CommLostRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking)) * infos.CommHalfWLRate,
            SComm = infos.CommLostRate * betTrans.SDiscount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking)) * infos.CommHalfWLRate,
            PlayerComm = infos.CommLostRate * betTrans.PlayerDiscount * betTrans.Stake * infos.CommHalfWLRate
        };
    }

    private static Commission ModdsLowerThanNegativeQuarter(BetTran betTrans, PrepareBetTran infos)
    {
        return new Commission()
        {
            Comm = infos.CommLostRate * betTrans.Discount * betTrans.Stake * (1 - betTrans.APositionTaking),
            AComm = infos.CommLostRate * betTrans.ADiscount * betTrans.Stake * (1 - (betTrans.MPositionTaking + betTrans.APositionTaking)),
            SComm = infos.CommLostRate * betTrans.SDiscount * betTrans.Stake * (1 - (betTrans.SPositionTaking + betTrans.MPositionTaking + betTrans.APositionTaking)),
            PlayerComm = infos.CommLostRate * betTrans.PlayerDiscount * betTrans.Stake
        };
    }
}