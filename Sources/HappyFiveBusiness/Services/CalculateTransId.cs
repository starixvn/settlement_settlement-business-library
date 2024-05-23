using Settlement.Business.Core.Models;

namespace HappyFiveBusiness.Services;

public static class CalculateTransId
{
    private const short InvalidBetDaqId = -2;

    private const byte DefaultRuben = 1;

    public static void Execute(IEnumerable<BetTransaction> listBetTrans, List<Transaction> listTrans)
    {
        var validBetTrans = listBetTrans.Where(b => b.BetDaqId != InvalidBetDaqId);
        foreach (var betTrans in validBetTrans)
        {
            var transInfo = listTrans.FirstOrDefault(t => t.TransId == betTrans.TransId);
            if (transInfo == null)
            {
                continue;
            }

            CalculateBetTrans(betTrans, transInfo);
            // var (requestUpdate, transactionInfo) = CalculateBetTrans(validBetTx, transInfo);
            //resultCalculated.Requests.Add(requestUpdate);
            //resultCalculated.Infos.Add(transactionInfo);
        }
    }

    private static void CalculateBetTrans(BetTransaction betTrans, Transaction trans)
    {
        var prepareData = new PrepareBetTransInfo(betTrans, trans);
        betTrans.MOdds = trans.MOdds;
        betTrans.StatusWinLost = CalculateFunction.GetStatusWL(trans.MOdds);
        betTrans.ExtraInfo = CalculateFunction.GetExtraInfo(betTrans, trans);

        if (prepareData.IsUpdVoid)
        {
            betTrans.WinLost = CalculateFunction.WinLost(trans, betTrans, prepareData.Odds);
            betTrans.AWinLost = CalculateFunction.AWinLost(trans, betTrans, prepareData.Odds);
            betTrans.MWinLost = CalculateFunction.MWinLost(trans, betTrans, prepareData.Odds);
            betTrans.SWinLost = CalculateFunction.SWinLost(trans, betTrans, prepareData.Odds);
            betTrans.PlayerComm = CalculateFunction.PlayerComm(betTrans, trans.MOdds, prepareData);
            betTrans.Comm = CalculateFunction.Comm(betTrans, trans.MOdds, prepareData);
            betTrans.AComm = CalculateFunction.AComm(betTrans, trans.MOdds, prepareData);
            betTrans.SComm = CalculateFunction.SComm(betTrans, trans.MOdds, prepareData);
            betTrans.Status = prepareData.Status;
            betTrans.StatusId = prepareData.StatusId;
            betTrans.Ruben = DefaultRuben;
            betTrans.SettlementTime = CalculateFunction.GetSettlementTime(betTrans, betTrans.WinLost, prepareData.StatusId);
        }

        // For logging
        // var transactionInfo = CreateInfoService.CreateTransactionInfo(betTrans, requestUpdateBetTrans);

        // return (requestUpdateBetTrans, transactionInfo);
    }
}