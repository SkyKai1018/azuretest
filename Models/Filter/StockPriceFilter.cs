namespace azuretest.Models;

public class StockPriceFilter : Filter
{
    public ComparisonOperators ComparisonOperators { get; set; }
    public double Price { get; set; }

    public override string FilterType
    {
        get
        {
            return "StockPriceFilter"; // Return the string value, not the class name
        }
    }
    public override string GetDescription()
    {
        return string.Format("最新股價 {0} {1}", ComparisonOperators.ToSymbol(), Price.ToString());
    }

    public override List<IIdentifiable> Execute(List<TradingData> tradingDatas)
    {
        var latestTradingDataPerStock = tradingDatas
            .GroupBy(td => td.StockId)
            .Select(g => g.OrderByDescending(td => td.Date).FirstOrDefault())
            .ToList();

        Func<TradingData, bool> predicate = td => td != null && GetPredicate(td.ClosePrice);

        var filteredStocks = latestTradingDataPerStock
            .Where(predicate)
            .Select(td => td.Stock)
            .Cast<IIdentifiable>()
            .ToList();

        return filteredStocks;
    }

    private bool GetPredicate(decimal closePrice)
    {
        return ComparisonOperators switch
        {
            ComparisonOperators.GreaterThan => (double)closePrice > Price,
            ComparisonOperators.LessThan => (double)closePrice < Price,
            ComparisonOperators.GreaterThanOrEqualTo => (double)closePrice >= Price,
            ComparisonOperators.LessThanOrEqualTo => (double)closePrice <= Price,
            ComparisonOperators.EqualTo => (double)closePrice == Price,
            _ => throw new InvalidOperationException($"Unsupported operator {ComparisonOperators}")
        };
    }
}

