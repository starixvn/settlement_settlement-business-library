namespace Settlement.Business.Core.Models;
public class Transaction
{
    public long TransId { get; set; }

    public decimal MOdds { get; set; }

    public byte OddsType { get; set; }

    public int Srecommend { get; set; }

    public short BetType { get; set; }

    public bool IsMarked { get; set; }

    public byte DeadHeat { get; set; }

    public decimal? WinLost { get; set; }

    public decimal ActualPO { get; set; }

    public bool IsMaxPayout { get; set; }
}
