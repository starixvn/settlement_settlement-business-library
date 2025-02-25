namespace Settlement.Business.Core.Requests;

public class TicketRequest
{
    public int MatchId { get; set; }

    public int AdminId { get; set; }

    public int SportType { get; set; }

    public string Resources { get; set; }

    public string Action { get; set; }

    public int ActionId { get; set; }

    public int MinCustId { get; set; }

    public long MinCustId_MinTransId { get; set; }

    public long MinCustId_MaxTransId { get; set; }

    public int MaxCustId { get; set; }

    public long MaxCustId_MinTransId { get; set; }

    public long MaxCustId_MaxTransId { get; set; }

    public string DbServer { get; set; }
}