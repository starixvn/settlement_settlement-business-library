namespace Settlement.Business.Requests;

public class UpdateTicketRequest
{
    public int MatchId { get; set; }

    public string Action { get; set; }

    public byte EventTable { get; set; }

    public bool Is3rd { get; set; }

    public bool IsCalculateBalance { get; set; }

    public byte BalanceType { get; set; }

    public short CWLBettype { get; set; }

    public bool SettleByApp { get; set; }

    public string TransJson { get; set; }

    public string DbServer { get; set; }
}