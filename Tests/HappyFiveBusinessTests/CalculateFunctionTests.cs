using HappyFiveBusiness;
using Settlement.Business.Core.Models;

namespace HappyFiveBusinessTests;
public class CalculateFunctionTests
{
    private readonly BetTransaction betTrans;

    private readonly Transaction tx;

    private readonly PrepareBetTranInfo model;

    public CalculateFunctionTests()
    {
        betTrans = new BetTransaction();
        tx = new Transaction();
        model = new PrepareBetTranInfo(betTrans, tx);
    }
}
