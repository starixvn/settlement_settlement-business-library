using Settlement.Business.Core.Models;
using Settlement.Business.Models;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Services;

public interface PrepareBetTranService
{
    PrepareBetTran CreateValue(BetTran betTran, List<WinLimit> maxWinLimit);
}

public class PrepareBetTranServiceImp(WinLostService winLostService) : PrepareBetTranService
{
    public PrepareBetTran CreateValue(BetTran betTran, List<WinLimit> maxWinLimit)
    {
        var (statusId, statusName) = Status(betTran.Modds);
        var (isApplyMaxPayout, maxWinLost) = winLostService.GetWinLost(betTran, maxWinLimit);

        return new PrepareBetTran
        {
            CommWinRate = betTran.ComStatus.Equals("1") ? 0 : 1,
            CommLostRate = betTran.ComStatus.Equals("1") ? 2 : 1,
            CommHalfWLRate = 0.5m, // Need to check: CASE WHEN sr.isCN88 = 1 AND sr.isMajorGroup = 1 THEN 1 ELSE 0.5
            Odds = Odds(betTran.Odds, betTran.OddsType),
            StatusId = statusId,
            Status = statusName,
            IsUpdated = betTran.StatusId != StatusId.Void && !betTran.IsMarked,
            IsApplyMaxPayout = isApplyMaxPayout,
            MaxWinLost = maxWinLost,
            WinLost = WinLost(betTran)
        };
    }

    private static decimal Odds(decimal odds, short oddsType)
    {
        if (odds < 0)
        {
            return 1;
        }

        if (oddsType == 1)
        {
            return odds - 1;
        }

        return odds;
    }

    private static (byte, string) Status(decimal modds)
    {
        return modds switch
        {
            > 0 => (StatusId.Won, StatusName.Won),
            0 => (StatusId.Draw, StatusName.Draw),
            _ => (StatusId.Lose, StatusName.Lose)
        };
    }

    private static decimal WinLost(BetTran betTran)
    {
        if (betTran is { WinLost: not null, IsApplyMaxPayout: true })
        {
            return (decimal)betTran.WinLost;
        }

        if (betTran.Modds > 0)
        {
            return betTran.DeadHeat > 0 ?
                (betTran.Odds / betTran.DeadHeat + 1) * betTran.Stake
                : betTran.ActualStake + betTran.Stake * betTran.Modds;
        }

        return 0;
    }
}