namespace Settlement.Business.App;

public class InputRequest
{
    public int EventId { get; set; }

    public byte Racers { get; set; }

    public int AdminId { get; set; }

    public int SportType { get; set; } = 164;

    public string Action { get; set; } = "CAL";

    public int ActionId { get; set; }

    public string Ip { get; set; } = string.Empty;
}