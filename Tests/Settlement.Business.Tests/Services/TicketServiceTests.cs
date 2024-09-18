using Fanex.Data.Repository;
using NSubstitute;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Requests;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class TicketServiceTests
{
    private readonly IDynamicRepository repository;

    private readonly TicketService service;

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
}