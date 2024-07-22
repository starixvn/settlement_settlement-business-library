namespace Settlement.Business.App;

public class InputRequest
{
    public int MatchId { get; set; }

    public int AdminId { get; set; }

    public int SportType { get; set; } = 164;

    public string Action { get; set; } = "CAL";

    public int ActionId { get; set; }

    public string Ip { get; set; } = string.Empty;
}