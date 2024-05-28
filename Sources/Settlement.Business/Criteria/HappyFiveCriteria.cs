using Fanex.Data;

namespace Settlement.Business.Criteria;

public class HappyFiveCriteria : NonQueryCommand
{
    public string BetTrans { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_UpdateBetTransHappy5_{DbServer}";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(BetTrans);
}