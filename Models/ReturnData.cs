using System;
namespace azureTest.Models
{
    public class ReturnData
    {
        public ReturnData(double totalInvestment, double finalMarketValue, double totalReturn, double returnRate, int days)
        {
            TotalInvestment = totalInvestment;
            FinalMarketValue = finalMarketValue;
            TotalReturn = totalReturn;
            ReturnRate = returnRate;
            Days = days;
        }
        public ReturnData(string stockId, double totalInvestment, double finalMarketValue, double totalReturn, double returnRate, int days)
        {
            StockId = stockId;
            TotalInvestment = totalInvestment;
            FinalMarketValue = finalMarketValue;
            TotalReturn = totalReturn;
            ReturnRate = returnRate;
            Days = days;
        }

        public string StockId { get; set; }
        public double TotalInvestment { get; set; }
        public double FinalMarketValue { get; set; }
        public double TotalReturn { get; set; }
        public double ReturnRate { get; set; }
        public double Days { get; set; }
    }
}

