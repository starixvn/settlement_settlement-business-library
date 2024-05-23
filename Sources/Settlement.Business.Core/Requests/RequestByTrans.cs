using Settlement.Business.Core.Helper;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Core.Requests;

public class RequestByTrans : RequestUpdateBetTrans
{
    private const byte DefaultRuben = 1;

    public RequestByTrans(BetTransaction betTransaction, Transaction tx, PrepareBetTranInfo prepareData) : base(betTransaction, tx)
    {
        WinLost = WinLostHelper.WinLost(tx, betTransaction, prepareData.Odds);
        AWinLost = WinLostHelper.AWinLost(tx, betTransaction, prepareData.Odds);
        MWinLost = WinLostHelper.MWinLost(tx, betTransaction, prepareData.Odds);
        SWinLost = WinLostHelper.SWinLost(tx, betTransaction, prepareData.Odds);

        PlayerComm = CommHelper.PlayerComm(betTransaction, tx.MOdds, prepareData);
        Comm = CommHelper.Comm(betTransaction, tx.MOdds, prepareData);
        AComm = CommHelper.AComm(betTransaction, tx.MOdds, prepareData);
        SComm = CommHelper.SComm(betTransaction, tx.MOdds, prepareData);

        Status = prepareData.Status;
        StatusId = prepareData.StatusId;
        Ruben = DefaultRuben;
        SettlementTime = StatusHelper.GetSettlementTime(betTransaction, WinLost, StatusId);
    }
}