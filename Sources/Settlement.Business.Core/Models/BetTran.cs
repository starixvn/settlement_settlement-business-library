using System.Text.Json.Serialization;

namespace Settlement.Business.Core.Models;

public class BetTran
{
    [JsonPropertyName("t")]
    public long TransId { get; set; }

    [JsonPropertyName("c")]
    public int CustId { get; set; }

    [JsonIgnore]
    public byte Currency { get; set; }

    [JsonPropertyName("r")]
    public byte? Ruben { get; set; }

    [JsonPropertyName("sti")]
    public byte StatusId { get; set; }

    [JsonPropertyName("sw")]
    public byte StatusWinLost { get; set; }

    [JsonIgnore]
    public byte WalletType { get; set; }

    [JsonPropertyName("tm")]
    public DateTime SettlementTime { get; set; }

    [JsonIgnore]
    public DateTime WinLostDate { get; set; }

    [JsonPropertyName("st")]
    public decimal Stake { get; set; }

    [JsonPropertyName("aco")]
    public decimal? AComm { get; set; }

    [JsonIgnore]
    public decimal ActualStake { get; set; }

    [JsonIgnore]
    public decimal ADiscount { get; set; }

    [JsonIgnore]
    public decimal APositionTaking { get; set; }

    [JsonPropertyName("awi")]
    public decimal? AWinLost { get; set; }

    [JsonPropertyName("co")]
    public decimal? Comm { get; set; }

    [JsonIgnore]
    public decimal Discount { get; set; }

    [JsonPropertyName("mo")]
    public decimal Modds { get; set; }

    [JsonIgnore]
    public decimal MPositionTaking { get; set; }

    [JsonPropertyName("mwi")]
    public decimal? MWinLost { get; set; }

    [JsonPropertyName("o")]
    public decimal Odds { get; set; }

    [JsonIgnore]
    public decimal OddsSpread { get; set; }

    [JsonPropertyName("pco")]
    public decimal? PlayerComm { get; set; }

    [JsonIgnore]
    public decimal PlayerDiscount { get; set; }

    [JsonPropertyName("sco")]
    public decimal SComm { get; set; }

    [JsonIgnore]
    public decimal SDiscount { get; set; }

    [JsonIgnore]
    public decimal SPositionTaking { get; set; }

    [JsonPropertyName("swi")]
    public decimal? SWinLost { get; set; }

    [JsonIgnore]
    public decimal TPositionTaking { get; set; }

    [JsonPropertyName("wi")]
    public decimal? WinLost { get; set; }

    [JsonIgnore]
    public int SRecommend { get; set; }

    [JsonIgnore]
    public int BonusId { get; set; }

    [JsonIgnore]
    public long RefNo { get; set; }

    [JsonIgnore]
    public long BetId { get; set; }

    [JsonIgnore]
    public short BetType { get; set; }

    [JsonIgnore]
    public string BetTeam { get; set; }

    [JsonIgnore]
    public string ComStatus { get; set; }

    [JsonPropertyName("e")]
    public string? ExtraInfo { get; set; }

    [JsonPropertyName("stn")]
    public string? Status { get; set; }

    [JsonIgnore]
    public int ProfileId { get; set; }

    [JsonIgnore]
    public short OddsType { get; set; }

    [JsonIgnore]
    public bool IsMarked { get; set; }

    [JsonIgnore]
    public decimal ActualPayout { get; set; }

    [JsonIgnore]
    public bool IsApplyMaxPayout { get; set; }

    [JsonIgnore]
    public byte DeadHeat { get; set; }
}