namespace Settlement.Business.Models;

public class BetTranBatch
{
    public int BatchNo { get; set; }

    public int MinCustId { get; set; }

    public long MinCustId_MinTransId { get; set; }

    public long MinCustId_MaxTransId { get; set; }

    public int MaxCustId { get; set; }

    public long MaxCustId_MinTransId { get; set; }

    public long MaxCustId_MaxTransId { get; set; }
}