using Fanex.Data;
using Settlement.Business.Core.Constants;

namespace Settlement.Business.Criteria;

public class GetTicketCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public int SportType { get; set; }

    public string Resources { get; set; }

    public string Action { get; set; }

    public int MinCustId { get; set; }

    public long MinCustId_MinTransId { get; set; }

    public long MinCustId_MaxTransId { get; set; }

    public int MaxCustId { get; set; }

    public long MaxCustId_MinTransId { get; set; }

    public long MaxCustId_MaxTransId { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public bool IsCentral => string.Equals(DbServer, DBNameAlias.Central, StringComparison.OrdinalIgnoreCase);

    public override string GetSettingKey() => $"NAP_Admin_Settlement_Ticket_Sel_{DbServer}";

    public override bool IsValid() => MatchId > 0;
}