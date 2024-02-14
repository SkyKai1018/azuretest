using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using azuretest.Data;
using azuretest.Models;

namespace StockBacktesting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region public

        #region HttpGet
        [HttpGet]
        public IActionResult GetStockData(string stockId)
        {
            var tradingDatas = _context.TradingDatas.Where(r => r.StockId == int.Parse(stockId)).ToList();

            var json = JsonSerializer.Serialize(tradingDatas, new JsonSerializerOptions { WriteIndented = true });
            return Ok(json);
        }

        [HttpGet("search/{query}")]
        public IActionResult SearchStocks(string query)
        {
            var matchedStocks = _context.TradingDatas.Select(record => record.StockId.ToString()).Distinct().ToList().Where(s => s.StartsWith(query)).ToList();


            // 序列化過濾後的紀錄為JSON
            var json = JsonSerializer.Serialize(matchedStocks, new JsonSerializerOptions { WriteIndented = true });
            return Ok(json);

        }

        [HttpGet("GetCalculateReturn")]
        public IActionResult GetCalculateReturn(string stockId, DateTime startDate, DateTime endDate, int specificDay)
        {

            var filteredRecords = _context.TradingDatas.Where(r => r.StockId.ToString() == stockId && r.Date >= startDate && r.Date <= endDate).ToList();

            var json = JsonSerializer.Serialize(CalculateReturnBySpecificDayOfMonth(filteredRecords, specificDay), new JsonSerializerOptions { WriteIndented = true });
            return Ok(json);

        }

        [HttpGet("GetCalculateReturnDayOfWeek")]
        public IActionResult GetCalculateReturnByDayOfWeek(DateTime startDate, DateTime endDate, DayOfWeek specificDayOfWeek)
        {
            var filteredRecords = _context.TradingDatas.Where(r => r.StockId == 2330 && r.Date >= startDate && r.Date <= endDate).ToList();

            var json = JsonSerializer.Serialize(CalculateReturnBySpecificDayOfWeek(filteredRecords, specificDayOfWeek), new JsonSerializerOptions { WriteIndented = true });
            return Ok(json);

        }

        [HttpPost("StartBacktest")]
        public async Task<IActionResult> StartBacktest([FromBody] BacktestRequest request)
        {
            try
            {
               var stockIdSet = new HashSet<int>(request.StockIds.Select(item =>
               {
                    if (int.TryParse(item, out int stockId)) return stockId;
                    return -1; // 返回一个无效的股票ID，稍后会被过滤掉
               }));

               var stocks = _context.Stocks
                    .Where(s => stockIdSet.Contains(s.StockId))
                    .Include(s => s.TradingDatas.Where(td => td.Date >= request.StartDate && td.Date <= request.EndDate))
                    .Include(s => s.EarningsDistributions.Where(ed => ed.Date >= request.StartDate && ed.Date <= request.EndDate))
                    .ToList();

               var result = stocks.AsParallel().Select(stockMatch =>
               {
                   var filteredRecords = stockMatch.TradingDatas.ToList();
                   return CalculateReturnBySpecificDayOfMonth(filteredRecords, request.SpecificDay);
               }).ToList();

                var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        public class BacktestRequest
        {
            public List<string> StockIds { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int SpecificDay { get; set; }
        }

        #endregion HttpGet

        #endregion

        #region Private

        #region Method

        ReturnData CalculateReturnBySpecificDayOfMonth(List<TradingData> data, int purchaseDay)
        {
            try
            {
                double totalShares = 0;
                double totalInvestment = 0;
                double investmentPerSpecificDay = 100;

                var filteredData = data;

                int lastMonth = -1;
                bool purchasedThisMonth = false;
                int days = 0;

                foreach (var day in filteredData)
                {
                    if (day.Date.Month != lastMonth)
                    {
                        // 重置標記，新的一個月
                        purchasedThisMonth = false;
                        lastMonth = day.Date.Month;
                    }

                    if (!purchasedThisMonth && (day.Date.Day >= purchaseDay || day.Date.Month != lastMonth))
                    {
                        // 購買操作
                        double sharesBought = investmentPerSpecificDay / (double)day.OpenPrice;
                        totalShares += sharesBought;
                        totalInvestment += investmentPerSpecificDay;
                        purchasedThisMonth = true; // 標記當月已購買
                        days++;
                    }
                }

                double finalMarketValue = totalShares * (double)filteredData.Last().ClosePrice;
                double totalReturn = finalMarketValue - totalInvestment;
                double returnRate = (totalReturn / totalInvestment) * 100;

                return new ReturnData(filteredData.First().StockId.ToString(), totalInvestment, finalMarketValue, totalReturn, returnRate, days);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return null;
            }
        }

        ReturnData CalculateReturnByDaily(List<TradingData> data)
        {
            double totalShares = 0;
            double totalInvestment = 0;
            double investmentPerDay = 100;

            var startDate = new DateTime(2020, 1, 1);
            var endDate = new DateTime(2024, 1, 2);

            var filteredData = data.Where(day => day.Date >= startDate && day.Date <= endDate).ToList();

            foreach (var day in filteredData)
            {
                double sharesBought = investmentPerDay / (double)day.OpenPrice;
                totalShares += sharesBought;
                totalInvestment += investmentPerDay;
            }

            double finalMarketValue = totalShares * (double)data.Last().ClosePrice;
            double totalReturn = finalMarketValue - totalInvestment;
            double returnRate = (totalReturn / totalInvestment) * 100;

            return new ReturnData(totalInvestment, finalMarketValue, totalReturn, returnRate, filteredData.Count);
        }

        ReturnData CalculateReturnBySpecificDayOfWeek(List<TradingData> data, DayOfWeek specificDayOfWeek)
        {
            double totalShares = 0;
            double totalInvestment = 0;
            double investmentPerDay = 100;

            // 篩選出特定星期的數據
            var filteredData = data.Where(day => day.Date.DayOfWeek == specificDayOfWeek).ToList();

            foreach (var day in filteredData)
            {
                double sharesBought = investmentPerDay / (double)day.OpenPrice;
                totalShares += sharesBought;
                totalInvestment += investmentPerDay;
            }

            decimal? lastClosePrice = filteredData.LastOrDefault()?.ClosePrice;
            decimal totalSharesAsDecimal = (decimal)totalShares;
            double finalMarketValue = (double)(totalSharesAsDecimal * (lastClosePrice ?? 0m));
            double totalReturn = finalMarketValue - totalInvestment;
            double returnRate = (totalInvestment > 0) ? (totalReturn / totalInvestment) * 100 : 0;

            return new ReturnData(totalInvestment, finalMarketValue, totalReturn, returnRate, filteredData.Count);
        }
        #endregion Method

        #endregion Private

    }
}