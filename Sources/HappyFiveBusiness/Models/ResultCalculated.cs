using HappyFiveBusiness.Requests;
using Settlement.Business.Core.Models;

namespace HappyFiveBusiness.Models;
public class ResultCalculated
{
    public List<RequestUpdateBetTrans> Requests { get; set; }

    public List<TransactionInfo> Infos { get; set; }
}
