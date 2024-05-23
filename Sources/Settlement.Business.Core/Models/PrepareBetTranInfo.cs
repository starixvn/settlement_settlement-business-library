using Settlement.Business.Core.Helper;

namespace Settlement.Business.Core.Models;

public class PrepareBetTranInfo(BetTransaction betTrans, Transaction tx)
{
    public readonly byte CommWinRate = CommHelper.CommWinRate(betTrans.ComStatus);

    public readonly byte CommLostRate = CommHelper.CommLostRate(betTrans.ComStatus);

    public readonly decimal Odds = StatusHelper.GetOdds(betTrans, tx);

    public readonly byte StatusId = StatusHelper.GetStatusId(tx.MOdds);

    public readonly string Status = StatusHelper.GetStatus(tx.MOdds);

    public readonly bool IsUpdVoid = StatusHelper.IsUpdVoid(betTrans, tx);
}