using Fanex.Settlement.Converter.Services;
using Settlement.Business.Core.Models;
using Settlement.Business.Core.Requests;
using Settlement.Business.Data.Criteria;

namespace Settlement.Business.Data.Services;

public interface HistoryData
{
    Task<Result> Insert(TicketRequest request, IEnumerable<Ticket> tickets);
}

public class HistoryDataImp(
    XmlConvertService xmlConvertService,
    IDynamicRepository repository) : HistoryData
{
    public async Task<Result> Insert(TicketRequest request, IEnumerable<Ticket> tickets)
    {
        var infos = CreateXmlObject(tickets, request);

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

    private static Root<MaxPayout> CreateXmlObject(IEnumerable<Ticket> tickets, TicketRequest request)
    {
        return new Root<MaxPayout>
        {
            Rows = tickets.Where(t => t.IsApplyMaxPayout).Select(t => new MaxPayout
            {
                CustId = t.CustId,
                ProductId = request.SportType,
                GroupId = 0,
                LevelId = 255,
                ActionId = 255,
                BetType = t.BetType,
                TransId = t.TransId,
                WinLostDate = t.TransDate,
                CreateBy = request.ActionId,
                WinLost = (decimal)t.MaxWinLimit!,
                ActualPayout = (decimal)t.ActualPayout!
            }).ToList()
        };
    }
}