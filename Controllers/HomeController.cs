using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using azuretest.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace azuretest.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFilterService _filterService;
    private readonly ICookieService _cookieService;


    public HomeController(ILogger<HomeController> logger, IFilterService filterService,ICookieService cookieService)
    {
        _logger = logger;
        _filterService = filterService;
        _cookieService = cookieService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Filter()
    {
        return View(_cookieService.GetFiltersFromCookie());
    }

    public IActionResult BackTest()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddStockPriceFilter(StockPriceFilter filter)
    {
        return AddFilter(filter);
    }

    [HttpPost]
    public IActionResult AddRaiseFilter(RiseFilter filter)
    {
        return AddFilter(filter);
    }

    [HttpPost]
    public IActionResult AddFallFilter(FallFilter filter)
    {
        return AddFilter(filter);
    }

    [HttpPost]
    public IActionResult AddDaysChangeFilter(DaysChangeFilter filter)
    {
        return AddFilter(filter);
    }


    [HttpPost("{id}")]
    public IActionResult DeleteFilterStrategy(int id)
    {
        var filters = _cookieService.GetFiltersFromCookie();
        filters.Remove(filters[id]);
        _cookieService.SaveFiltersToCookie(filters);

        return View("Filter", filters);
    }

    [HttpPost]
    public IActionResult StartFilter()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var filters = _cookieService.GetFiltersFromCookie();
        ViewData["GroupedRecords"] = _filterService.StartFilter(filters);
        stopwatch.Stop();
        _cookieService.SaveFiltersToCookie(filters);

        ViewData["second"] = stopwatch.Elapsed.TotalSeconds;

        return View("Filter", filters);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    IActionResult AddFilter<TFilter>(TFilter filter) where TFilter : Filter
    {
        if (ModelState.IsValid)
        {
            var filters = _cookieService.GetFiltersFromCookie();
            filters.Add(filter);
            _cookieService.SaveFiltersToCookie(filters);

            return View("Filter", filters);
        }

        return View("Filter", null);
    }
}