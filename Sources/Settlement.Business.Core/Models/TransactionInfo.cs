using System.Text.Json.Serialization;

namespace Settlement.Business.Core.Models;

public class TransactionInfo
{
    [JsonPropertyName("t")]
    public long TransId { get; set; }

    [JsonIgnore]
    public int CustId { get; set; }

    [JsonIgnore]
    public decimal Balance { get; set; }

    [JsonPropertyName("m")]
    public decimal Modds { get; set; }

    [JsonPropertyName("o")]
    public decimal Odds { get; set; }

    [JsonPropertyName("statuswl")]
    public byte StatusWinLost { get; set; }

    [JsonPropertyName("st")]
    public DateTime? SettlementTime { get; set; }

    [JsonPropertyName("w")]
    public decimal? WinLost { get; set; }

    [JsonPropertyName("aw")]
    public decimal AWinLost { get; set; }

    [JsonPropertyName("mw")]
    public decimal MWinLost { get; set; }

    [JsonPropertyName("sw")]
    public decimal SWinLost { get; set; }

    [JsonPropertyName("pc")]
    public decimal PlayerComm { get; set; }

    [JsonPropertyName("c")]
    public decimal Comm { get; set; }

    [JsonPropertyName("ac")]
    public decimal AComm { get; set; }

    [JsonPropertyName("sc")]
    public decimal SComm { get; set; }

    [JsonPropertyName("s")]
    public string Status { get; set; }

    [JsonPropertyName("sid")]
    public byte StatusID { get; set; }

    [JsonPropertyName("r")]
    public byte Ruben { get; set; }

    [JsonPropertyName("ex")]
    public string ExtraInfo { get; set; }

    [JsonPropertyName("bef_w")]
    public decimal WinLostBefore { get; set; }

    [JsonPropertyName("bef_sid")]
    public byte StatusIdBefore { get; set; }

    [JsonPropertyName("bt")]
    public short BetType { get; set; }

    [JsonPropertyName("bi")]
    public long BetId { get; set; }

    [JsonPropertyName("rf")]
    public long RefNo { get; set; }

    [JsonIgnore]
    public byte BeforeRuben { get; set; }

    [JsonIgnore]
    public DateTime WinLostDate { get; set; }

    [JsonIgnore]
    public decimal ActualStake { get; set; }
}
