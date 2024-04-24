using HappyFiveBusiness.Models;
using HappyFiveBusiness.Requests;
using Settlement.Business.Core.Models;

namespace HappyFiveBusiness.Services;
public static class CalculateTransId
{
    private const short InvalidBetDaqId = -1;

    public static ResultCalculated Execute(IEnumerable<BetTransaction> listBetTrans, List<Transaction> listTrans)
    {
        var validBetTrans = listBetTrans.Where(b => b.BetDaqId != InvalidBetDaqId);
        var resultCalculated = new ResultCalculated();
        foreach (var validBetTx in validBetTrans)
        {
            var transInfo = listTrans.FirstOrDefault(t => t.TransId == validBetTx.TransId);
            if (transInfo == null)
            {
                continue;
            }

            var (requestUpdate, transactionInfo) = CalculateBetTrans(validBetTx, transInfo);
            resultCalculated.Requests.Add(requestUpdate);
            resultCalculated.Infos.Add(transactionInfo);
        }

        return resultCalculated;
    }


    private static (RequestUpdateBetTrans, TransactionInfo) CalculateBetTrans(BetTransaction betTrans, Transaction trans)
    {
        var prepareBetTransInfo = new PrepareBetTranInfo(betTrans, trans);
        RequestUpdateBetTrans requestUpdateBetTrans;

        if (prepareBetTransInfo.IsUpdVoid)
        {
            requestUpdateBetTrans = new RequestByTrans(betTrans, trans, prepareBetTransInfo);
        }
        else
        {
            requestUpdateBetTrans = new RequestByBetTrans(betTrans, trans);
        }

        var transactionInfo = CreateInfoService.CreateTransactionInfo(betTrans, requestUpdateBetTrans);

        return (requestUpdateBetTrans, transactionInfo);
    }

}
