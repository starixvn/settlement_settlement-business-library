namespace Settlement.Business.Core.Models;

public class Customer
{
    [JsonPropertyName("c")]
    public int CustId { get; set; }

    [JsonPropertyName("cur")]
    public byte Currency { get; set; }
}