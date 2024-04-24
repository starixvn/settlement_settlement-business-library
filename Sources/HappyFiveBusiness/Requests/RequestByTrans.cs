using HappyFiveBusiness.Services;
using Settlement.Business.Core.Models;

namespace HappyFiveBusiness.Requests;
public class RequestByTrans : RequestUpdateBetTrans
{
    private const byte DefaultRuben = 1;
    public RequestByTrans(BetTransaction betTransaction, Transaction tx, PrepareBetTranInfo prepareData) : base(betTransaction, tx)
    {
        WinLost = CalculateFunction.WinLost(tx, betTransaction, prepareData.Odds);
        AWinLost = CalculateFunction.AWinLost(tx, betTransaction, prepareData.Odds);
        MWinLost = CalculateFunction.MWinLost(tx, betTransaction, prepareData.Odds);
        SWinLost = CalculateFunction.SWinLost(tx, betTransaction, prepareData.Odds);
        PlayerComm = CalculateFunction.PlayerComm(betTransaction, tx.MOdds, prepareData);
        Comm = CalculateFunction.Comm(betTransaction, tx.MOdds, prepareData);
        AComm = CalculateFunction.AComm(betTransaction, tx.MOdds, prepareData);
        SComm = CalculateFunction.SComm(betTransaction, tx.MOdds, prepareData);
        Status = prepareData.Status;
        StatusId = prepareData.StatusId;
        Ruben = DefaultRuben;
        SettlementTime = CalculateFunction.GetSettlementTime(betTransaction, WinLost, StatusId);
    }
}
