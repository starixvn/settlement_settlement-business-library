using Settlement.Business.Core.Helper;
using Settlement.Business.Core.Models;
using Settlement.Business.Models;

namespace Settlement.Business.Services;

public interface GenerateBetTranService
{
    Task<List<BetTran>> Happy5(List<BetTran> betTrans);
}

public class GenerateBetTranServiceImp(
    ModdsService moddsService,
    WinLimitService winLimitService,
    PrepareBetTranService prepareBetTranService) : GenerateBetTranService
{
    private const byte DefaultRuben = 1;

    public async Task<List<BetTran>> Happy5(List<BetTran> betTrans)
    {
        betTrans.ForEach(item => item.Modds = moddsService.Happy5(item.ProfileId, item.BetTeam, "", ""));

        var maxWinLimit = (await winLimitService.GetMaxWinLimit(betTrans)).ToList();

        betTrans.ForEach(item => SetValue(item, maxWinLimit));

        return betTrans;
    }

    private void SetValue(BetTran betTran, List<WinLimit> maxWinLimit)
    {
        var prepareBetTran = prepareBetTranService.CreateValue(betTran, maxWinLimit);
        betTran.IsApplyMaxPayout = prepareBetTran.IsApplyMaxPayout;
        betTran.WinLost = prepareBetTran.MaxWinLost;

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
            betTran.SettlementTime = StatusHelper.GetSettlementTime(betTran, prepareBetTran.WinLost, prepareBetTran.StatusId);

            betTran.Ruben = DefaultRuben;
            betTran.WinLost = prepareBetTran.WinLost;
            betTran.StatusId = prepareBetTran.StatusId;
            betTran.Status = prepareBetTran.Status;
        }
        // Todo: betTran.Odds = ...
    }
}