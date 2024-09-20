namespace Settlement.Business.Requests;

public class UpdateTicketRequest
{
    public int MatchId { get; set; }

    public string Action { get; set; }

    public int ActionId { get; set; }

    public byte EventTable { get; set; }

    public bool HasNoBalanceNegative { get; set; }

    public string TransJson { get; set; }

    public string DbServer { get; set; }
}