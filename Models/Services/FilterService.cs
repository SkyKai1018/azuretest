using System.Diagnostics;
using azuretest.Data;
using azuretest.Models;
using Microsoft.EntityFrameworkCore;

public class FilterService : IFilterService
{
    private readonly ApplicationDbContext _context;

    public FilterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddFilter(Filter filter)
    {
        DataStorage.Filters.Add(filter);
    }

    public List<Stock> StartFilter()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // 一次性加载所有需要的数据
        var tradingDatas = _context.TradingDatas
            .Include(td => td.Stock)
            .ToList();

        var earningsDistributions = _context.EarningsDistributions
            .Include(ed => ed.Stock)
            .ToList();

        var output = new List<IIdentifiable>();

        // 使用HashSet提高性能
        HashSet<int> stockIds = new HashSet<int>();

        // 避免重复的数据库操作
        foreach (var item in DataStorage.Filters)
        {
            if (output.Count == 0)
            {
                output = item.Execute(tradingDatas);
            }
            else
            {
                var filterData = tradingDatas
                    .Where(td => stockIds.Contains(td.StockId))
                    .ToList();
                output = item.Execute(filterData);
            }
            // 更新stockIds
            stockIds = new HashSet<int>(output.Cast<Stock>().Select(s => s.StockId));
            item.result = item.Execute(tradingDatas).Count;
        }

        stopwatch.Stop();

        return output.Cast<Stock>().ToList();
    }

    public void DeleteFilterStrategy(int id)
    {
        DataStorage.Filters.Remove(DataStorage.Filters[id]);
    }
}