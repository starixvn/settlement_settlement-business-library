using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using NSubstitute;
using Settlement.Business.Core.Models;
using Settlement.Business.Core.Requests;
using Settlement.Business.Data.Criteria;
using Settlement.Business.Data.Services;

namespace Settlement.Business.Data.Tests;

public class HistoryDataTests
{
    private readonly IDynamicRepository repository;

    private readonly XmlConvertService xmlConvertService;

    private readonly HistoryDataImp service;

    public HistoryDataTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        xmlConvertService = Substitute.For<XmlConvertService>();
        service = new HistoryDataImp(xmlConvertService, repository);
    }

    [Fact]
    public async Task Insert_ExistedMaxPayoutTicket_ReturnsInsertHistory()
    {
        var tickets = new List<Ticket>{
            new()
            {
                MaxWinLimit = 1,
                ActualPayout = 1,
                IsApplyMaxPayout = true
            }
        };
        var request = new TicketRequest
        {
            AdminId = 1
        };

        xmlConvertService.Serialize(Arg.Any<Root<MaxPayout>>()).Returns("XML");
        await service.Insert(request, tickets);
        await repository.Received(1).ExecuteAsync(Arg.Any<InsertHistoryDataCriteria>());
    }

    [Fact]
    public async Task Insert_NotExistedMaxPayoutTicket_ReturnsNotReceiveInsertHistory()
    {
        var tickets = new List<Ticket>{
            new()
            {
                MaxWinLimit = 1,
                ActualPayout = 1,
                IsApplyMaxPayout = false
            }
        };
        var request = new TicketRequest
        {
            AdminId = 1
        };

        xmlConvertService.Serialize(Arg.Any<Root<MaxPayout>>()).Returns("XML");
        await service.Insert(request, tickets);
        await repository.DidNotReceive().ExecuteAsync(Arg.Any<InsertHistoryDataCriteria>());
    }
}