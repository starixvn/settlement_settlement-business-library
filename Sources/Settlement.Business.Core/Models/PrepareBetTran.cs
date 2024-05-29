namespace Settlement.Business.Core.Models;

public class PrepareBetTran
{
    public decimal CommWinRate { get; set; }

    public decimal CommLostRate { get; set; }

    public decimal CommHalfWLRate { get; set; }

    public decimal Odds { get; set; }

    public byte StatusId { get; set; }

    public string Status { get; set; }

    public bool IsUpdated { get; set; }

    public bool IsApplyMaxPayout { get; set; }

    public decimal? MaxWinLost { get; set; }

    public decimal WinLost { get; set; }
}