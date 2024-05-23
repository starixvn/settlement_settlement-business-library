using Settlement.Business.Core.Helper;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface GenerateBetTransService
{
    List<BetTransaction> Happy5(List<BetTransaction> betTrans);
}

public class GenerateBetTransServiceImp(
    ModdsService moddsService,
    CustomerService customerService,
    WinLimitService winLimitService,
    WinLostService winLostService) : GenerateBetTransService
{
    private const byte DefaultRuben = 1;

    public List<BetTransaction> Happy5(List<BetTransaction> betTrans)
    {
        betTrans.ToList().ForEach(item => item.Modds = moddsService.Happy5(item.ProfileId, item.BetTeam, "", ""));

        var listCustIds = customerService.GetCustomersByModds(betTrans);
        var getMaxWinLimit = winLimitService.GetMaxWinLimit(listCustIds);

        foreach (var item in betTrans)
        {
            var (isApplyMaxPayout, winLost) = winLostService.UpdateWinLost(item, getMaxWinLimit);
            item.IsApplyMaxPayout = isApplyMaxPayout;
            item.WinLost = winLost;

            SetValue(item);
        }

        return betTrans;
    }

    private static void SetValue(BetTransaction betTran)
    {
        var prepareData = new PrepareBetTransInfo(betTran);
        betTran.StatusWinLost = StatusHelper.GetStatusWinLost(betTran.Modds);
        betTran.ExtraInfo = StatusHelper.GetExtraInfo(betTran);

        if (prepareData.IsUpdated)
        {
            betTran.AWinLost = WinLostHelper.AWinLost(betTran, prepareData.Odds);
            betTran.MWinLost = WinLostHelper.MWinLost(betTran, prepareData.Odds);
            betTran.SWinLost = WinLostHelper.SWinLost(betTran, prepareData.Odds);
            betTran.PlayerComm = CommHelper.PlayerComm(betTran, betTran.Modds, prepareData);
            betTran.Comm = CommHelper.Comm(betTran, betTran.Modds, prepareData);
            betTran.AComm = CommHelper.AComm(betTran, betTran.Modds, prepareData);
            betTran.SComm = CommHelper.SComm(betTran, betTran.Modds, prepareData);
            betTran.Status = prepareData.Status;
            betTran.StatusId = prepareData.StatusId;
            betTran.Ruben = DefaultRuben;
            betTran.SettlementTime = StatusHelper.GetSettlementTime(betTran, prepareData.WinLost, prepareData.StatusId);
            betTran.WinLost = prepareData.WinLost;
        }
        // Todo: betTran.Odds = ...
    }
}