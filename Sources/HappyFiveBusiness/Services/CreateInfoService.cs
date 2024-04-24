using HappyFiveBusiness.Requests;
using Settlement.Business.Core.Models;

namespace HappyFiveBusiness.Services;
public static class CreateInfoService
{
    public static TransactionInfo CreateTransactionInfo(BetTransaction betTrans, RequestUpdateBetTrans request)
    {
        return new TransactionInfo
        {
            TransId = betTrans.TransId,
            CustId = betTrans.CustId,
            Balance = request.WinLost - betTrans.WinLost + (request.PlayerComm - betTrans.PlayerComm),
            Modds = request.MOdds,
            Odds = betTrans.Odds,
            StatusWinLost = request.StatusWinLost,
            SettlementTime = DateTime.Compare(betTrans.SettlementTime, request.SettlementTime) == 0 ? null : request.SettlementTime,
            WinLost = request.WinLost,
            AWinLost = request.AWinLost,
            MWinLost = request.MWinLost,
            SWinLost = request.SWinLost,
            PlayerComm = request.PlayerComm,
            Comm = request.Comm,
            AComm = request.AComm,
            SComm = request.SComm,
            Status = request.Status,
            StatusID = request.StatusId,
            Ruben = request.Ruben,
            BeforeRuben = betTrans.Ruben,
            ExtraInfo = request.ExtraInfo,
            WinLostBefore = betTrans.WinLost,
            StatusIdBefore = betTrans.StatusId,
            BetType = betTrans.BetType,
            BetId = betTrans.BetId,
            RefNo = betTrans.RefNo,
            WinLostDate = betTrans.WinLostDate,
            ActualStake = betTrans.ActualStake
        };
    }
}
