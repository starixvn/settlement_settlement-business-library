using Settlement.Business.Core.Models;
using Settlement.Business.Requests;
using System.Text.Json;

namespace Settlement.Business.Services.HappyFive;

public interface ProcessService
{
    Task<Result> Calc(TicketRequest request, List<Resources> resources);

    Task<Result> Refund(TicketRequest request, List<Resources> resources);
}

public class ProcessServiceImp(
    TicketService ticketService,
    CommandService generateTicketService,
    InsertHistoryData insertHistoryData) : ProcessService
{
    public async Task<Result> Calc(TicketRequest request, List<Resources> resources)
    {
        var tickets = await ticketService.GetTickets(request);
        var ticketsAfterCalculate = await generateTicketService.Calc(tickets, resources);

        var ticketsJson = JsonSerializer.Serialize(ticketsAfterCalculate);
        var updateRequest = CreateUpdateRequest(request, ticketsJson);
        var updateTicketResult = await ticketService.UpdateTicket(updateRequest);

        if (updateTicketResult.IsSuccessful)
        {
            await insertHistoryData.InsertHappy5(tickets, request.AdminId);
        }

        return updateTicketResult;
    }

    public async Task<Result> Refund(TicketRequest request, List<Resources> resources)
    {
        var tickets = await ticketService.GetTickets(request);
        var ticketsAfterRefund = generateTicketService.Refund(tickets, resources);
        var ticketsJson = JsonSerializer.Serialize(ticketsAfterRefund);
        var updateRequest = CreateUpdateRequest(request, ticketsJson);
        return await ticketService.UpdateTicket(updateRequest);
    }

    private static UpdateTicketRequest CreateUpdateRequest(TicketRequest request, string ticketsJson)
    {
        return new UpdateTicketRequest
        {
            MatchId = request.MatchId,
            TransJson = ticketsJson,
            Action = request.Action,
            DbServer = request.DbServer
        };
    }
}