using HappyFiveBusiness.Services;
using Settlement.Business.Core.Models;

namespace HappyFiveBusiness.Requests;
public class RequestUpdateBetTrans
{
    protected RequestUpdateBetTrans(BetTransaction betTrans, Transaction tx)
    {
        MOdds = tx.MOdds;
        StatusWinLost = CalculateFunction.GetStatusWL(tx.MOdds);
        ExtraInfo = CalculateFunction.GetExtraInfo(betTrans, tx);
    }

    public readonly decimal MOdds;

    public readonly byte StatusWinLost;

    public DateTime SettlementTime { get; protected set; }

    public decimal WinLost { get; protected set; }

    public decimal AWinLost { get; protected set; }

    public decimal MWinLost { get; protected set; }

    public decimal SWinLost { get; protected set; }

    public decimal PlayerComm { get; protected set; }

    public decimal Comm { get; protected set; }

    public decimal AComm { get; protected set; }

    public decimal SComm { get; protected set; }

    public string Status { get; protected set; }

    public byte StatusId { get; protected set; }

    public byte Ruben { get; protected set; }

    public string ExtraInfo { get; protected set; }
}
