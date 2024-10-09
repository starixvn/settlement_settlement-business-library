namespace Settlement.Business.Core.Models;

public class RaceInfo
{
    public int EventId { get; set; }

    public string EventStatus { get; set; }

    public DateTime EventTime { get; set; }

    public short SportType { get; set; }

    public int LeagueId { get; set; }

    public byte Racers { get; set; }

    public string VendorMatchId { get; set; }

    public string Distance { get; set; }
}