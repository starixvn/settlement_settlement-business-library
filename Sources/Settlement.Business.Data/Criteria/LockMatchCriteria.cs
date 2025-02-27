using Fanex.Data;
using Settlement.Business.Core.Constants;

namespace Settlement.Business.Data.Criteria;

public class LockMatchCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public byte Lock { get; set; }

    public int Type { get; set; } = LockMatchType.Match;

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_Admin_Settlement_Match_Locking_{DbServer}";

    public override bool IsValid() => MatchId > 0;
}