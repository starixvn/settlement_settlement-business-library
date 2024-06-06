using Settlement.Business.Core.Helper;
using Settlement.Business.Core.Models;
using Settlement.Business.Models;

namespace Settlement.Business.Services;

public interface GenerateBetTranService
{
    Task<List<BetTran>> Happy5(List<BetTran> betTrans, List<Resources> resources);
}

public class GenerateBetTranServiceImp(
    ModdsService moddsService,
    WinLimitService winLimitService,
    PrepareBetTranService prepareBetTranService) : GenerateBetTranService
{
    private const byte DefaultRuben = 1;

    public async Task<List<BetTran>> Happy5(List<BetTran> betTrans, List<Resources> resources)
    {
        betTrans.ForEach(item =>
        {
            var resource = resources.Find(r => r.ProfileId == item.ProfileId) ?? new Resources();
            item.Modds = moddsService.Happy5(item.ProfileId, item.BetTeam, resource);
            item.OddsType = resource.OddsType;
        });
        var maxWinLimit = (await winLimitService.GetMaxWinLimit(betTrans)).ToList();

        betTrans.ForEach(item => SetValue(item, maxWinLimit));

        return betTrans;
    }

    private void SetValue(BetTran betTran, List<WinLimit> maxWinLimit)
    {
        var prepareBetTran = prepareBetTranService.CreateValue(betTran, maxWinLimit);
        betTran.IsApplyMaxPayout = prepareBetTran.IsApplyMaxPayout;
        betTran.StatusWinLost = StatusHelper.GetStatusWinLost(betTran.Modds);
        betTran.ExtraInfo = StatusHelper.GetExtraInfo(betTran);

        if (prepareBetTran.IsUpdated)
        {
            var commission = CommHelper.CreateCommission(betTran, betTran.Modds, prepareBetTran);
            var winlost = WinLostHelper.CreateWinLost(betTran, prepareBetTran);
            betTran.AWinLost = winlost.AWinLost;
            betTran.MWinLost = winlost.MWinLost;
            betTran.SWinLost = winlost.SWinLost;
            betTran.PlayerComm = commission.PlayerComm;
            betTran.Comm = commission.Comm;
            betTran.AComm = commission.AComm;
            betTran.SComm = commission.SComm;
            betTran.SettlementTime = StatusHelper.GetSettlementTime(betTran, prepareBetTran.WinLost, prepareBetTran.StatusId);

            betTran.Ruben = DefaultRuben;
            betTran.WinLost = winlost.WinLost;
            betTran.StatusId = prepareBetTran.StatusId;
            betTran.Status = prepareBetTran.Status;
        }
        // Todo: betTran.Odds = ...
    }
}