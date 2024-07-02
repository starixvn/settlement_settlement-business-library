using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using NSubstitute;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Services;

namespace Settlement.Business.Tests.Services;

public class InsertHistoryDataTests
{
    private readonly IDynamicRepository repository;

    private readonly XmlConvertService xmlConvertService;

    private readonly InsertHistoryData service;

    public InsertHistoryDataTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        xmlConvertService = Substitute.For<XmlConvertService>();
        service = new InsertHistoryDataImp(xmlConvertService, repository);
    }

    [Fact]
    public async Task InsertHappy5_ExistedMaxPayoutTicket_ReturnsInsertHistory()
    {
        var tickets = new List<Ticket>{
            new()
            {
                MaxWinLimit = 1,
                ActualPayout = 1,
                IsApplyMaxPayout = true
            }
        };
        xmlConvertService.Serialize(Arg.Any<Root<MaxPayout>>()).Returns("XML");
        await service.InsertHappy5(tickets, 1);
        await repository.Received(1).ExecuteAsync(Arg.Any<InsertHistoryDataCriteria>());
    }

    [Fact]
    public async Task InsertHappy5_NotExistedMaxPayoutTicket_ReturnsNotReceiveInsertHistory()
    {
        var tickets = new List<Ticket>{
            new()
            {
                MaxWinLimit = 1,
                ActualPayout = 1,
                IsApplyMaxPayout = false
            }
        };
        xmlConvertService.Serialize(Arg.Any<Root<MaxPayout>>()).Returns("XML");
        await service.InsertHappy5(tickets, 1);
        await repository.DidNotReceive().ExecuteAsync(Arg.Any<InsertHistoryDataCriteria>());
    }
}