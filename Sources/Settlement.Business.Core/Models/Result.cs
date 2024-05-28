namespace Settlement.Business.Core.Models;

public class Result
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public bool IsSuccessful => string.IsNullOrEmpty(ErrorMessage) && ErrorCode == 0;
}