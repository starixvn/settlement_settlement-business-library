using HappyFiveBusiness.Services;
using Settlement.Business.Core.Models;

namespace HappyFiveBusiness;

public class PrepareBetTranInfo
{
    public readonly byte CommWinRate;

    public readonly byte CommLostRate;

    public readonly decimal Odds;

    public readonly byte StatusId;

    public readonly string Status;

    public readonly bool IsUpdVoid;

    public PrepareBetTranInfo(BetTransaction betTrans, Transaction tx)
    {
        CommWinRate = CalculateFunction.CommWinRate(betTrans.ComStatus);
        CommLostRate = CalculateFunction.CommLostRate(betTrans.ComStatus);
        IsUpdVoid = CalculateFunction.IsUpdVoid(betTrans, tx);
        Odds = CalculateFunction.GetOdds(betTrans, tx);
        StatusId = CalculateFunction.GetStatusId(tx.MOdds);
        Status = CalculateFunction.GetStatus(tx.MOdds);
    }
}
