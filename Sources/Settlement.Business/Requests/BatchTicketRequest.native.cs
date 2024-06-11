using Settlement.Business.Models;

namespace Settlement.Business.Requests;

public class BatchTicketRequest
{
    public int MatchId { get; set; }

    public int AdminId { get; set; }

    public int SportType { get; set; }

    public List<Resources> Resources { get; set; }

    public string Action { get; set; }

    public int LastCustId { get; set; }

    public long LastTransId { get; set; }

    public int BatchSize { get; set; }

    public int BatchTotal { get; set; }

    public string DbServer { get; set; }
}