using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using azuretest.Data;
using Microsoft.EntityFrameworkCore;
using azuretest.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace azuretest.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFilterService _filterService;


    public HomeController(ILogger<HomeController> logger, IFilterService filterService)
    {
        _logger = logger;
        _filterService = filterService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Filter()
    {
        return View(new List<Filter>());
    }

    public IActionResult BackTest()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddStockPriceFilter(StockPriceFilter filter)
    {
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
        if (ModelState.IsValid)
        {
            _filterService.AddFilter(filter);

            return View("Filter", DataStorage.Filters);
        }

        return View("Filter", null);
    }

    [HttpPost]
    public IActionResult AddDaysChangeFilter(DaysChangeFilter filter)
    {
        if (ModelState.IsValid)
        {
            _filterService.AddFilter(filter);

            return View("Filter", DataStorage.Filters);
        }

        return View("Filter", null);
    }


    [HttpPost("{id}")]
    public IActionResult DeleteFilterStrategy(int id)
    {
        DataStorage.Filters.Remove(DataStorage.Filters[id]);

        return View("Filter", DataStorage.Filters);
    }

    [HttpPost]
    public IActionResult StartFilter()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        ViewData["GroupedRecords"] = _filterService.StartFilter();

        stopwatch.Stop();

        ViewData["second"] = stopwatch.Elapsed.TotalSeconds;

        return View("Filter", DataStorage.Filters);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}