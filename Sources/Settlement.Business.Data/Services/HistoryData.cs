using Fanex.Settlement.Converter.Services;
using Settlement.Business.Core.Models;
using Settlement.Business.Core.Requests;
using Settlement.Business.Data.Criteria;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Data.Services;

public interface HistoryData
{
    Task<Result> Insert(TicketRequest request, IEnumerable<Ticket> tickets);
}

public class HistoryDataImp(
    XmlConvertService xmlConvertService,
    IDynamicRepository repository)
    : HistoryData
{
    private const int ErrorCode = 1;

    public async Task<Result> Insert(TicketRequest request, IEnumerable<Ticket> tickets)
    {
        var infos = CreateXmlObject(tickets, request);

        if (infos.Rows.Count == 0)
        {
            return new Result();
        }

        try
        {
            var xml = xmlConvertService.Serialize(infos);
            var criteria = new InsertHistoryDataCriteria { XmlData = xml };

            await repository.ExecuteAsync(criteria);
            return new Result();
        }
        catch (Exception exception)
        {
            return CreateErrorResult(request, exception);
        }
    }

    private static Result CreateErrorResult(TicketRequest request, Exception exception)
    {
        return new Result
        {
            ErrCode = ErrorCode,
            ErrMessage = $"Match Id: {request.MatchId} - SiteDb: {request.DbServer} - Message: {exception.Message}"
        };
    }

    private static Root<MaxPayout> CreateXmlObject(IEnumerable<Ticket> tickets, TicketRequest request)
    {
        var validTickets = tickets.Where(IsValidTicketForHistory);

        return new Root<MaxPayout>
        {
            Rows = validTickets.Select(t => CreateMaxPayout(t, request)).ToList()
        };
    }

    private static bool IsValidTicketForHistory(Ticket ticket)
    {
        return ticket.IsApplyMaxPayout &&
               ticket.StatusId != StatusId.Void &&
               !ticket.IsMarked;
    }

    private static MaxPayout CreateMaxPayout(Ticket ticket, TicketRequest request)
    {
        return new MaxPayout
        {
            CustId = ticket.CustId,
            ProductId = request.SportType,
            GroupId = 0,
            LevelId = 255,
            ActionId = 255,
            BetType = ticket.BetType,
            TransId = ticket.TransId,
            WinLostDate = ticket.TransDate,
            CreateBy = request.ActionId,
            WinLost = (decimal)ticket.MaxWinLimit!,
            ActualPayout = (decimal)ticket.ActualPayout!
        };
    }
}