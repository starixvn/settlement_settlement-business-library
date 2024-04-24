using HappyFiveBusiness.Services;

namespace HappyFiveBusinessTests;

public class CalculateMOddsTests
{
    [Theory]
    [InlineData(8101, "b", "53", "5", 1)]
    [InlineData(8101, "b", "51", "53", -1)]
    [InlineData(8101, "s", "51", "53", 1)]
    [InlineData(8101, "s", "53", "5", -1)]
    [InlineData(8102, "o", "1", "5", 1)]
    [InlineData(8102, "o", "2", "5", -1)]
    [InlineData(8102, "e", "2", "5", 1)]
    [InlineData(8102, "e", "1", "5", -1)]
    [InlineData(8104, "mo", "14", "5", 1)]
    [InlineData(8104, "mo", "5", "14", -1)]
    [InlineData(8104, "me", "33", "5", -1)]
    [InlineData(8104, "me", "5", "33", 1)]
    [InlineData(8105, "bo", "52", "33", -1)]
    [InlineData(8105, "bo", "54", "33", -1)]
    [InlineData(8105, "bo", "55", "33", 1)]
    [InlineData(8105, "be", "54", "33", 1)]
    [InlineData(8105, "be", "55", "33", -1)]
    [InlineData(8105, "so", "51", "33", 1)]
    [InlineData(8105, "so", "52", "33", -1)]
    [InlineData(8105, "se", "52", "33", 1)]
    [InlineData(8105, "se", "51", "33", -1)]
    public void GetMOdds_Always_ReturnCorrectValue(int profileID, string betTeam, string homeValue, string awayValue, int expected)
    {

        var actual = CalculateMOdds.Execute(profileID, betTeam, homeValue, awayValue);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(8103, "sp", 15, 33)]
    [InlineData(8103, "su", 34, 52)]
    [InlineData(8103, "au", 53, 71)]
    [InlineData(8103, "wi", 72, 90)]
    public void GetMOdds_ProfileThird_ReturnCorrectValue(int profileID, string betTeam, int min, int max)
    {
        var actual = true;
        for (var i = min; i <= max; i++)
        {
            var homeValue = i.ToString();
            actual = actual && CalculateMOdds.Execute(profileID, betTeam, homeValue, "20") == 1;
        }

        Assert.True(actual);
    }
}