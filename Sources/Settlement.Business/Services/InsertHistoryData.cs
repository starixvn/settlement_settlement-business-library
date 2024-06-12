using Fanex.Settlement.Converter.Services;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;

namespace Settlement.Business.Services;

public interface InsertHistoryData
{
    Task InsertHappy5(IEnumerable<Ticket> tickets, int adminId);
}

public class InsertHistoryDataImp(
    XmlConvertService xmlConvertService,
    IDynamicRepository repository) : InsertHistoryData
{
    public async Task InsertHappy5(IEnumerable<Ticket> tickets, int adminId)
    {
        var infos = CreateXmlObject(tickets, adminId);

        if (infos.Rows.Count != 0)
        {
            var xml = xmlConvertService.Serialize(infos);

            var criteria = new InsertHistoryDataCriteria()
            {
                XmlData = xml
            };

            await repository.ExecuteAsync(criteria);
        }
    }

    private static Root<MaxPayout> CreateXmlObject(IEnumerable<Ticket> tickets, int adminId)
    {
        return new Root<MaxPayout>()
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