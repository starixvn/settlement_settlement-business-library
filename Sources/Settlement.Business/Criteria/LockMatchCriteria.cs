using Fanex.Data;

namespace Settlement.Business.Criteria;

public class LockMatchCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public byte Lock { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_Admin_Settlement_Match_Locking_{DbServer}";

    public override bool IsValid() => MatchId > 0;
}