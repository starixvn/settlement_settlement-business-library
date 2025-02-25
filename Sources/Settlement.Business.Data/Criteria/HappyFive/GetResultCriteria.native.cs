namespace Settlement.Business.Data.Criteria.HappyFive;

public class GetResultCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public string Action { get; set; }

    public override string GetSettingKey() => "NAP_Set_Happy5_Result_Sel";

    public override bool IsValid() => MatchId > 0 && !string.IsNullOrWhiteSpace(Action);
}