namespace Settlement.Business.Data.Criteria;
public class InsertHistoryDataCriteria : INonQueryCommand
{
    public string XmlData { get; set; }

    public string GetSettingKey() => "Task_Winlimit_InsertHistoryData";

    public bool IsValid() => !string.IsNullOrEmpty(XmlData);
}
