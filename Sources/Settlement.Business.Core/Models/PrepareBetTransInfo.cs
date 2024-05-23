using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Core.Models;

public class PrepareBetTransInfo(BetTransaction betTran)
{
    public decimal CommWinRate => betTran.ComStatus.Equals("1") ? 0 : 1;

    public decimal CommLostRate => betTran.ComStatus.Equals("1") ? 2 : 1;

    public decimal CommHalfWLRate => 0.5m; // Need to check: CASE WHEN sr.isCN88 = 1 AND sr.isMajorGroup = 1 THEN 1 ELSE 0.5

    public decimal Odds
    {
        get
        {
            if (betTran.Odds < 0)
            {
                return 1;
            }

            if (betTran.OddsType == 1)
            {
                return betTran.Odds - 1;
            }

            return betTran.Odds;
        }
    }

    public byte StatusId
    {
        get
        {
            return betTran.Modds switch
            {
                > 0 => Constants.Status.StatusId.Won,
                0 => Constants.Status.StatusId.Draw,
                _ => Constants.Status.StatusId.Lose
            };
        }
    }

    public string Status
    {
        get
        {
            return betTran.Modds switch
            {
                > 0 => StatusName.Won,
                0 => StatusName.Draw,
                _ => StatusName.Lose
            };
        }
    }

    public bool IsUpdated => betTran.StatusId != Constants.Status.StatusId.Void && !betTran.IsMarked;

    public decimal WinLost
    {
        get
        {
            if (betTran is { WinLost: not null, IsApplyMaxPayout: true })
            {
                return (decimal)betTran.WinLost;
            }

            if (betTran.Modds > 0)
            {
                return betTran.DeadHeat > 0 ?
                    (Odds / betTran.DeadHeat + 1) * betTran.Stake
                    : betTran.ActualStake + betTran.Stake * betTran.Modds;
            }

            return 0;
        }
    }
}