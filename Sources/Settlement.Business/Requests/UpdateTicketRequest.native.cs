namespace Settlement.Business.Requests;

public class UpdateTicketRequest
{
    public int MatchId { get; set; }

    public string Action { get; set; }

    public string TransJson { get; set; }

    public string DbServer { get; set; }
}