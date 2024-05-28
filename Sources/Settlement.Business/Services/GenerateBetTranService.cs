using Settlement.Business.Core.Helper;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface GenerateBetTranService
{
    List<BetTran> Happy5(List<BetTran> betTrans);
}

public class GenerateBetTranServiceImp(
    ModdsService moddsService,
    CustomerService customerService,
    WinLimitService winLimitService,
    PrepareBetTranService prepareBetTranService) : GenerateBetTranService
{
    private const byte DefaultRuben = 1;

    public List<BetTran> Happy5(List<BetTran> betTrans)
    {
        betTrans.ToList().ForEach(item => item.Modds = moddsService.Happy5(item.ProfileId, item.BetTeam, "", ""));

        var listCustIds = customerService.GetCustomersByModds(betTrans);
        var maxWinLimit = winLimitService.GetMaxWinLimit(listCustIds);

        foreach (var item in betTrans)
        {
            var (isApplyMaxPayout, winLost) = prepareBetTranService.GetWinLost(item, maxWinLimit);
            item.IsApplyMaxPayout = isApplyMaxPayout;
            item.WinLost = winLost;

            SetValue(item);
        }

        return betTrans;
    }

    private void SetValue(BetTran betTran)
    {
        var prepareBetTran = prepareBetTranService.CreateValue(betTran);
        betTran.StatusWinLost = StatusHelper.GetStatusWinLost(betTran.Modds);
        betTran.ExtraInfo = StatusHelper.GetExtraInfo(betTran);

        if (prepareBetTran.IsUpdated)
        {
            betTran.AWinLost = WinLostHelper.AWinLost(betTran, prepareBetTran.Odds);
            betTran.MWinLost = WinLostHelper.MWinLost(betTran, prepareBetTran.Odds);
            betTran.SWinLost = WinLostHelper.SWinLost(betTran, prepareBetTran.Odds);
            betTran.PlayerComm = CommHelper.PlayerComm(betTran, betTran.Modds, prepareBetTran);
            betTran.Comm = CommHelper.Comm(betTran, betTran.Modds, prepareBetTran);
            betTran.AComm = CommHelper.AComm(betTran, betTran.Modds, prepareBetTran);
            betTran.SComm = CommHelper.SComm(betTran, betTran.Modds, prepareBetTran);
            betTran.Status = prepareBetTran.Status;
            betTran.StatusId = prepareBetTran.StatusId;
            betTran.Ruben = DefaultRuben;
            betTran.SettlementTime = StatusHelper.GetSettlementTime(betTran, prepareBetTran.WinLost, prepareBetTran.StatusId);
            betTran.WinLost = prepareBetTran.WinLost;
        }
        // Todo: betTran.Odds = ...
    }
}