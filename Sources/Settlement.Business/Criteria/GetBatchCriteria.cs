using Fanex.Data;

namespace Settlement.Business.Criteria;

public class GetBatchCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public int SportType { get; set; }

    public string Resources { get; set; }

    public string Action { get; set; }

    public int LastCustId { get; set; }

    public long LastTransId { get; set; }

    public int BatchSize { get; set; }

    public int BatchTotal { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_Admin_Settlement_BatchTicket_Sel_{DbServer}";

    public override bool IsValid() => MatchId > 0;
}