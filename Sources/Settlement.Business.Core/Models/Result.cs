namespace Settlement.Business.Core.Models;

public class Result
{
    public int ErrCode { get; set; }

    public string ErrMessage { get; set; }

    public bool IsSuccessful => string.IsNullOrEmpty(ErrMessage) && ErrCode == 0;
}