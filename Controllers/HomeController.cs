using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using azureTest.Data;
using Microsoft.EntityFrameworkCore;
using azuretest.Models;

namespace azureTest.Controllers;

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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

