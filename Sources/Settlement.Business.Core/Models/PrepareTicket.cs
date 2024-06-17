namespace Settlement.Business.Core.Models;

public class PrepareTicket
{
    public decimal CommWinRate { get; set; }

    public decimal CommLostRate { get; set; }

    public decimal CommHalfWLRate { get; set; }

    public decimal Odds { get; set; }

    public byte StatusId { get; set; }

    public string Status { get; set; }

    public byte StatusWinLost { get; set; }

    public bool IsUpdated { get; set; }

    public bool IsApplyMaxPayout { get; set; }

    public decimal? MaxWinLost { get; set; }

    public decimal? WinLost { get; set; }

    public string ExtraInfo { get; set; }

    public DateTime? SettlementTime { get; set; }
}