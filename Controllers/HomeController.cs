using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using azuretest.Data;
using Microsoft.EntityFrameworkCore;
using azuretest.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace azuretest.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;


    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var groupedRecords = _context.TradingDatas
            .Include(td => td.Stock)  // 包括关联的Stock
            .GroupBy(r => r.StockId)
            .Select(group => group.OrderByDescending(r => r.Date).FirstOrDefault())
            .ToList();

        ViewData["Data"] = groupedRecords;

        return View();
    }

    public IActionResult Filter()
    {
        //後續可優化功能
        var groupedRecords = _context.TradingDatas
            .Include(td => td.Stock)  // 包括关联的Stock
            .GroupBy(r => r.StockId)
            .Select(group => group.OrderByDescending(r => r.Date).FirstOrDefault())
            .ToList();

        ViewData["GroupedRecords"] = groupedRecords;
        ViewData["FilterStrategy"] = new SelectList(Enum.GetValues(typeof(FilterStrategy)));

        return View(new List<Filter>());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddStockPriceFilter(StockPriceFilter filter)
    {
        ViewData["GroupedRecords"] = null;
        ViewData["FilterStrategy"] = null;

        if (ModelState.IsValid)
        {
            DataStorage.Filters.Add(filter);
            return View("Filter", DataStorage.Filters);
        }

        return View("Filter", null);
    }

    [HttpPost]
    public IActionResult AddRaiseFilter(RiseFilter filter)
    {
        ViewData["GroupedRecords"] = null;
        ViewData["FilterStrategy"] = null;

        if (ModelState.IsValid)
        {
            DataStorage.Filters.Add(filter);

            return View("Filter", DataStorage.Filters);
        }

        return View("Filter", null);
    }

    [HttpPost]
    public IActionResult AddFallFilter(FallFilter filter)
    {
        ViewData["GroupedRecords"] = null;
        ViewData["FilterStrategy"] = null;

        if (ModelState.IsValid)
        {
            DataStorage.Filters.Add(filter);
            return View("Filter", DataStorage.Filters);
        }

        return View("Filter", null);
    }

    [HttpPost]
    public IActionResult AddDaysChangeFilter(DaysChangeFilter filter)
    {
        ViewData["GroupedRecords"] = null;
        ViewData["FilterStrategy"] = null;

        if (ModelState.IsValid)
        {
            DataStorage.Filters.Add(filter);
            return View("Filter", DataStorage.Filters);
        }

        return View("Filter", null);
    }


    [HttpPost("{id}")]
    public IActionResult DeleteFilterStrategy(int id)
    {
        DataStorage.Filters.Remove(DataStorage.Filters[id]);
        ViewData["GroupedRecords"] = null;
        ViewData["FilterStrategy"] = null;

        return View("Filter", DataStorage.Filters);
    }

    [HttpPost]
    public IActionResult StartFilter()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        var TradingDatas = _context.TradingDatas
        .Include(td => td.Stock) // 包括关联的Stock
        .ToList(); // 把数据取出到内存中

        _context.EarningsDistributions
        .Include(td => td.Stock) // 包括关联的Stock
        .ToList();

        var OutPut = new List<IIdentifiable>();
        //計算每個篩選符合的家數
        foreach (var item in DataStorage.Filters)
        {
            if (OutPut.Count == 0) OutPut = item.Execute(TradingDatas);
            else
            {
                var stockIds = OutPut.Cast<Stock>().Select(s => s.StockId).ToList(); // 从stocks集合中提取所有的StockId

                var FilterData = TradingDatas
                    .Where(td => stockIds.Contains(td.StockId)) // 只选择those在stockIds集合中的TradingDatas
                    .OrderBy(td => td.StockId).ThenBy(td => td.Date) // 按StockId和Date排序
                    .ToList(); // 把数据取出到内存中
                OutPut = item.Execute(FilterData);
            }
            item.result = item.Execute(_context.TradingDatas.ToList()).Count;
        }

        stopwatch.Stop();
        ViewData["GroupedRecords"] = OutPut.Cast<Stock>().ToList();
        ViewData["second"] = stopwatch.Elapsed.TotalSeconds;

        return View("Filter", DataStorage.Filters);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

