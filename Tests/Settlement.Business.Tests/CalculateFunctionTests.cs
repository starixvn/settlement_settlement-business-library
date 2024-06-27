using Settlement.Business.Core.Models;

namespace HappyFiveBusinessTests;

public class CalculateFunctionTests
{
    private readonly BetTransaction betTrans;

    private readonly PrepareBetTransInfo model;

    public CalculateFunctionTests()
    {
        betTrans = new BetTransaction();
        model = new PrepareBetTransInfo(betTrans);
    }
}