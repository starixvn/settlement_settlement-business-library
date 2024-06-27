using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class ModdsValueServiceTests
{
    [Theory]
    [InlineData(8101, "1", "", 1)]
    [InlineData(8103, "3", "", 3)]
    [InlineData(8105, "5", "", 5)]
    [InlineData(8102, "6", "2", 0)]
    [InlineData(8104, "4", "2", 2)]
    [InlineData(111, "", "", -1)]
    public void GetValueByProfileIdOfHappy5_Always_ReturnCorrectValue(int profileId, string homeValue, string awayValue, int expected)
    {
        var service = new ModdsValueServiceImp();
        var actual = service.GetValueByProfileIdOfHappy5(profileId, homeValue, awayValue);

        Assert.Equal(expected, actual);
    }
}