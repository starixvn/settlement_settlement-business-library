using Settlement.Business.Core.Models;

namespace HappyFiveBusiness.Requests;
public class RequestByBetTrans : RequestUpdateBetTrans
{
    public RequestByBetTrans(BetTransaction betTransaction, Transaction tx) : base(betTransaction, tx)
    {
        SettlementTime = betTransaction.SettlementTime;
        WinLost = betTransaction.WinLost;
        AWinLost = betTransaction.AWinLost;
        MWinLost = betTransaction.MWinLost;
        SWinLost = betTransaction.SWinLost;
        PlayerComm = betTransaction.PlayerComm;
        Comm = betTransaction.Comm;
        AComm = betTransaction.AComm;
        SComm = betTransaction.SComm;
        Status = betTransaction.Status;
        StatusId = betTransaction.StatusId;
        Ruben = betTransaction.Ruben;
    }
}
