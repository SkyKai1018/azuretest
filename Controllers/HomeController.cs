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

    public IActionResult BackTest()
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
                //stockIds = new HashSet<int>(output.Cast<Stock>().Select(s => s.StockId));
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
        ViewData["GroupedRecords"] = output.Cast<Stock>().ToList();
        ViewData["second"] = stopwatch.Elapsed.TotalSeconds;

        return View("Filter", DataStorage.Filters);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

