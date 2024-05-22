namespace Settlement.Business.Core.Models;

public class MaxPayout(int siteDbGroupId, int siteId)
{
    public int SiteDbGroupId { get; set; } = siteDbGroupId;

    public int SiteId { get; set; } = siteId;
}