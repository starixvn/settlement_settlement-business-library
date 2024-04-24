using HappyFiveBusiness;
using Settlement.Business.Core.Models;

namespace HappyFiveBusinessTests;
public class PrepareBetTranInfoTests
{
    private readonly BetTransaction betTrans;

    private readonly Transaction tx;

    private readonly PrepareBetTranInfo model;

    public PrepareBetTranInfoTests()
    {
        betTrans = new BetTransaction();
        tx = new Transaction();
        model = new PrepareBetTranInfo(betTrans, tx);
    }
}
