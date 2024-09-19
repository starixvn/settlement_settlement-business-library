using Fanex.Settlement.Converter.Services;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Requests;

namespace Settlement.Business.Services;

public interface InsertHistoryData
{
    Task<Result> InsertHappy5(TicketRequest request, IEnumerable<Ticket> tickets);

    Task<Result> InsertVirtualRacing(TicketRequest request, IEnumerable<Ticket> tickets);
}

public class InsertHistoryDataImp(
    XmlConvertService xmlConvertService,
    IDynamicRepository repository) : InsertHistoryData
{
    public async Task<Result> InsertHappy5(TicketRequest request, IEnumerable<Ticket> tickets)
    {
        var infos = CreateXmlObject(tickets, request.AdminId);

        if (infos.Rows.Count != 0)
        {
            var xml = xmlConvertService.Serialize(infos);

            var criteria = new InsertHistoryDataCriteria
            {
                XmlData = xml
            };
            try
            {
                await repository.ExecuteAsync(criteria);
            }
            catch (Exception exception)
            {
                return new Result
                {
                    ErrCode = 1,
                    ErrMessage = $"Match Id: {request.MatchId} - SiteDb: {request.DbServer} - Message: {exception.Message}"
                };
            }
        }

        return new Result();
    }

    public async Task<Result> InsertVirtualRacing(TicketRequest request, IEnumerable<Ticket> tickets)
    {
        return new Result();
    }

    private static Root<MaxPayout> CreateXmlObject(IEnumerable<Ticket> tickets, int adminId)
    {
        return new Root<MaxPayout>
        {
            Rows = tickets.Where(t => t.IsApplyMaxPayout).Select(t => new MaxPayout
            {
                CustId = t.CustId,
                ProductId = Products.Happy5,
                GroupId = 0,
                LevelId = 255,
                ActionId = 255,
                BetType = t.BetType,
                TransId = t.TransId,
                WinLostDate = t.TransDate,
                CreateBy = adminId,
                WinLost = (decimal)t.MaxWinLimit!,
                ActualPayout = (decimal)t.ActualPayout!
            }).ToList()
        };
    }
}