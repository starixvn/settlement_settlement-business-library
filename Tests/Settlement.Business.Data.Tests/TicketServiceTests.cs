using Fanex.Data.Repository;
using NSubstitute;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Core.Requests;
using Settlement.Business.Data.Criteria;
using Settlement.Business.Data.Services;

namespace Settlement.Business.Data.Tests;

public class TicketServiceTests
{
    private readonly IDynamicRepository repository;

    private readonly TicketServiceImp service;

    public TicketServiceTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        service = new TicketServiceImp(repository);
    }

    [Fact]
    public async Task GetBatches_EmptyTickets_ReturnsLastCustIdIsZero()
    {
        repository.FetchAsync<TicketBatch>(Arg.Any<GetBatchTicketCriteria>()).Returns(new List<TicketBatch>());

        var request = new BatchTicketRequest
        {
            LastCustId = 1,
            LastTransId = 1,
            Resources = []
        };

        var actual = await service.GetBatches(request);

        Assert.Equal(0, request.LastCustId);
        Assert.Equal(0, request.LastTransId);
        Assert.True(actual.Count == 0);
    }

    [Fact]
    public async Task GetBatches_HaveTickets_ReturnsCorrectLastCustIdAndLastTransId()
    {
        var list = new List<TicketBatch>
        {
            new()
            {
                MaxCustId = 11,
                MaxCustId_MaxTransId = 11
            },
            new()
            {
                MaxCustId = 22,
                MaxCustId_MaxTransId = 22
            },
            new()
            {
                MaxCustId = 33,
                MaxCustId_MaxTransId = 44
            }
        };
        repository.FetchAsync<TicketBatch>(Arg.Any<GetBatchTicketCriteria>()).Returns(list);

        var request = new BatchTicketRequest
        {
            LastCustId = 1,
            LastTransId = 1,
            Resources = []
        };

        var actual = await service.GetBatches(request);

        Assert.Equal(33, request.LastCustId);
        Assert.Equal(44, request.LastTransId);
        Assert.True(actual.Count > 0);
    }

    [Theory]
    [InlineData(SportType.Happy5, 1, true, 81)]
    [InlineData(SportType.VirtualDogs, 4, false, 1201)]
    [InlineData(SportType.VirtualHorses, 4, false, 1201)]
    public async Task UpdateTicket_SportTypeIsVirtualRacing_ReturnsRightExpectedValue(int sportType, byte expectedEventTable, bool expectedHasNoBalanceNegative, short expectedCwlBettype)
    {
        await repository.ExecuteAsync(Arg.Any<UpdateOtherSportsCriteria>());

        var ticket = new TicketRequest
        {
            SportType = sportType
        };

        var actual = await service.UpdateTicket(ticket, []);

        await repository.Received(1).ExecuteAsync(Arg.Is<UpdateOtherSportsCriteria>(x =>
            x.EventTable == expectedEventTable
            && x.HasNoBalanceNegative == expectedHasNoBalanceNegative
            && x.CWLBettype == expectedCwlBettype));

        Assert.True(actual.IsSuccessful);
    }
}