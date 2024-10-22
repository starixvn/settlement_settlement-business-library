using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class ModdsServiceTests
{
    private readonly ModdsService service;

    public ModdsServiceTests()
    {
        var moddsValueService = new ModdsValueServiceImp();
        service = new ModdsServiceImp(moddsValueService);
    }

    [Theory]
    [InlineData(8101, "b", "53", "", 1)]
    [InlineData(8101, "b", "51", "", -1)]
    [InlineData(8101, "s", "51", "", 1)]
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
    [InlineData(111, "se", "51", "33", -1)]
    public void Happy5_Always_ReturnCorrectValue(int profileId, string betTeam, string homeValue, string awayValue, int expected)
    {
        var resource = new Resources { Result1 = homeValue, Result2 = awayValue };
        var actual = service.Happy5(profileId, betTeam, resource);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(8103, "sp", 15, 33)]
    [InlineData(8103, "su", 34, 52)]
    [InlineData(8103, "au", 53, 71)]
    [InlineData(8103, "wi", 72, 90)]
    public void Happy5_ProfileThird_ReturnCorrectValue(int profileId, string betTeam, int min, int max)
    {
        var actual = true;
        for (var i = min; i <= max; i++)
        {
            var homeValue = i.ToString();
            var resource = new Resources { Result1 = homeValue };
            actual = actual && service.Happy5(profileId, betTeam, resource) == 1;
        }

        Assert.True(actual);
    }

    [Theory]
    [InlineData(2, 2, 1, 1)]
    [InlineData(3, 3, 2, -1)]
    public void VirtualRacing_BetTypeEqual2770_ReturnsExpectedValue(int oddsId, int orid, byte placing, int expected)
    {
        var ticket = new Ticket
        {
            BetType = BetTypes.RacingWin,
            OddsId = oddsId,
        };

        var outright = new List<Outright>()
        {
           new()
           {
               Orid = orid,
               Placing = placing
           }
        };

        var actual = service.VirtualRacing(ticket, outright);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(2, 2, 1)]
    [InlineData(4, 3, -1)]
    public void VirtualRacing_BetTypeNotEqual2770_ReturnsExpectedValue(int oddsId, int orid, int expected)
    {
        var ticket = new Ticket
        {
            BetType = BetTypes.RacingPlace,
            OddsId = oddsId,
        };

        var outright = new List<Outright>()
        {
            new()
            {
                Orid = orid,
            }
        };

        var actual = service.VirtualRacing(ticket, outright);

        Assert.Equal(expected, actual);
    }
}