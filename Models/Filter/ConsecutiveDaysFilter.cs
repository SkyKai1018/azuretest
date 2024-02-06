using System.Collections.Concurrent;

namespace azuretest.Models;

public abstract class ConsecutiveDaysFilter : Filter
{
    public int Days { get; set; }

    protected abstract bool PriceComparison(decimal previousPrice, decimal currentPrice);

    //public override List<IIdentifiable> Execute(List<TradingData> tradingDatas)
    //{
    //    var matchingStocks = new List<IIdentifiable>();

    //    foreach (var group in tradingDatas.GroupBy(td => td.StockId))
    //    {
    //        int consecutiveDays = 0;
    //        TradingData previousData = null;

    //        foreach (var currentData in group.OrderByDescending(td => td.Date).Take(Days + 1))
    //        {
    //            if (previousData != null && PriceComparison(previousData.ClosePrice, currentData.ClosePrice))
    //            {
    //                consecutiveDays++;
    //            }
    //            else
    //            {
    //                consecutiveDays = 0; // Reset if the condition doesn't meet
    //            }

    //            if (consecutiveDays >= Days)
    //            {
    //                matchingStocks.Add(currentData.Stock);
    //                break; // This stock meets the condition, move to the next one
    //            }

    //            previousData = currentData;
    //        }
    //    }

    //    return matchingStocks;
    //}

    public override List<IIdentifiable> Execute(List<TradingData> tradingDatas)
    {
        var matchingStocks = new ConcurrentBag<IIdentifiable>();

        Parallel.ForEach(tradingDatas.GroupBy(td => td.StockId), (group) =>
        {
            int consecutiveDays = 0;
            TradingData previousData = null;

            foreach (var currentData in group.OrderByDescending(td => td.Date).Take(Days + 1))
            {
                if (previousData != null && PriceComparison(previousData.ClosePrice, currentData.ClosePrice))
                {
                    consecutiveDays++;
                }
                else
                {
                    consecutiveDays = 0; // Reset if the condition doesn't meet
                }

                if (consecutiveDays >= Days)
                {
                    matchingStocks.Add(currentData.Stock);
                    break; // This stock meets the condition, move to the next one
                }

                previousData = currentData;
            }
        });

        return matchingStocks.ToList();
    }

}
