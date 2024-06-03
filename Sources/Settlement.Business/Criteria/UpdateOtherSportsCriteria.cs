using Fanex.Data;

namespace Settlement.Business.Criteria;

public class UpdateOtherSportsCriteria : NonQueryCommand
{
    public int MatchId { get; set; }

    public string Action { get; set; }

    public byte EventTable { get; set; }

    public bool Is3rd { get; set; }

    public bool IsCalculateBalance { get; set; }

    public byte BalanceType { get; set; }

    public short CWLBettype { get; set; }

    public bool SettleByApp { get; set; }

    public string TransJson { get; set; }

    public string BroadcastSessionId { get; set; } = Guid.NewGuid().ToString();

    public int ErrCode { get; set; }

    public string ErrMessage { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_UpdateBetTransHappy5_{DbServer}";

    public override bool IsValid() => MatchId > 0;
}