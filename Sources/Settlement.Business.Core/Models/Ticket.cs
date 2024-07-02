namespace Settlement.Business.Core.Models;

public class Ticket
{
    [JsonPropertyName("t")]
    public long TransId { get; set; }

    [JsonPropertyName("c")]
    public int CustId { get; set; }

    [JsonPropertyName("r")]
    public byte? Ruben { get; set; }

    [JsonPropertyName("sti")]
    public byte StatusId { get; set; }

    [JsonPropertyName("sw")]
    public byte StatusWinLost { get; set; }

    [JsonPropertyName("wi")]
    public decimal? WinLost { get; set; }

    [JsonPropertyName("awi")]
    public decimal? AWinLost { get; set; }

    [JsonPropertyName("mwi")]
    public decimal? MWinLost { get; set; }

    [JsonPropertyName("swi")]
    public decimal? SWinLost { get; set; }

    [JsonPropertyName("pco")]
    public decimal? PlayerComm { get; set; }

    [JsonPropertyName("tm")]
    public bool IsUpdateSettlementTime { get; set; }

    [JsonPropertyName("st")]
    public decimal Stake { get; set; }

    [JsonPropertyName("co")]
    public decimal? Comm { get; set; }

    [JsonPropertyName("aco")]
    public decimal? AComm { get; set; }

    [JsonPropertyName("sco")]
    public decimal SComm { get; set; }

    [JsonPropertyName("o")]
    public decimal Odds { get; set; }

    [JsonPropertyName("mo")]
    public decimal Modds { get; set; }

    [JsonPropertyName("e")]
    public string ExtraInfo { get; set; }

    [JsonPropertyName("stn")]
    public string Status { get; set; }

    [JsonIgnore]
    public byte Currency { get; set; }

    [JsonIgnore]
    public decimal ActualStake { get; set; }

    [JsonIgnore]
    public decimal ADiscount { get; set; }

    [JsonIgnore]
    public decimal APositionTaking { get; set; }

    [JsonIgnore]
    public decimal Discount { get; set; }

    [JsonIgnore]
    public decimal MPositionTaking { get; set; }

    [JsonIgnore]
    public decimal PlayerDiscount { get; set; }

    [JsonIgnore]
    public decimal SDiscount { get; set; }

    [JsonIgnore]
    public decimal SPositionTaking { get; set; }

    [JsonIgnore]
    public short BetType { get; set; }

    [JsonIgnore]
    public string BetTeam { get; set; }

    [JsonIgnore]
    public string ComStatus { get; set; }

    [JsonIgnore]
    public int ProfileId { get; set; }

    [JsonIgnore]
    public short OddsType { get; set; }

    [JsonIgnore]
    public bool IsMarked { get; set; }

    [JsonIgnore]
    public decimal? ActualPayout { get; set; }

    [JsonIgnore]
    public bool IsApplyMaxPayout { get; set; }

    [JsonIgnore]
    public decimal? MaxWinLimit { get; set; }

    [JsonIgnore]
    public byte? DeadHeat { get; set; }

    [JsonIgnore]
    public DateTime TransDate { get; set; }
}