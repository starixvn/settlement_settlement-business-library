using Settlement.Business.Core.Models;

namespace HappyFiveBusinessTests;

public class CalculateTransIdTests
{
    private readonly BetTransaction betTrans = new();

    [Fact]
    public void WinLost_TxWinLostIsNotNullAndIsMaxPayout_ReturnFromTx()
    {
        betTrans.IsApplyMaxPayout = true;
        betTrans.WinLost = 10.5m;

        var actual = win.WinLost(betTrans, 10m);

        Assert.Equal(10.5m, actual);
    }

    [Fact]
    public void WinLost_TxMOddsLessThanZero_ReturnZero()
    {
        betTrans.IsApplyMaxPayout = false;
        betTrans.Modds = -1;

        var actual = CalculateFunction.WinLost(tx, betTrans, 10m);

        Assert.Equal(0, actual);
    }

    [Fact]
    public void WinLost_TxMOddsValidAndDeadHeatGreaterThanOne_ReturnExpectedValue()
    {
        betTrans.MOdds = 0.006m;
        betTrans.DeadHeat = 2;
        betTrans.Stake = 0.415m;
        betTrans.ActualStake = 0.000005m;
        var odds = 0.006m;

        var actual = CalculateFunction.WinLost(tx, betTrans, odds);

        Assert.Equal(0.0502M, actual);

        // (0.006 / 2 + 1) * 0.05
    }
}