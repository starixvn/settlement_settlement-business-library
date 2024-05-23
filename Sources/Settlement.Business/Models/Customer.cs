namespace Settlement.Business.Models;

public class Customer
{
    [JsonPropertyName("t")]
    public int CustId { get; set; }

    [JsonPropertyName("c")]
    public byte Currency { get; set; }
}