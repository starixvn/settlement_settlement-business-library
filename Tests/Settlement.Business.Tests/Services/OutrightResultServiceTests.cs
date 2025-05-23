using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class OutrightResultServiceTests
{
    private readonly OutrightResultServiceImp service = new();

    [Theory]
    [InlineData(SportType.VirtualDogs, 6, new[] { 1, 2 })]
    [InlineData(SportType.VirtualSteepleChase, 6, new[] { 1, 2 })]
    [InlineData(SportType.VirtualHarnessRacing, 6, new[] { 1, 2 })]
    [InlineData(SportType.VirtualMotorRacing, 6, new[] { 1, 2 })]
    [InlineData(SportType.VirtualDogs, 8, new[] { 1, 2, 3 })]
    [InlineData(SportType.VirtualSteepleChase, 8, new[] { 1, 2, 3 })]
    public void CorrectOutrightResult_WithValidPlacings_ReturnsFilteredOutright(int sportType, byte racers, int[] expectedPlacings)
    {
        // Arrange
        var outright = new List<Outright>
        {
            new() { Placing = 1 },
            new() { Placing = 2 },
            new() { Placing = 3 },
            new() { Placing = 4 }
        };

        // Act
        var actual = service.CorrectOutrightResult(sportType, racers, outright);

        // Assert
        Assert.All(actual, o => Assert.Contains(o.Placing, expectedPlacings));
    }
}