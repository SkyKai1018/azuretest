namespace azuretest.Models;

public class DaysChangeFilter : Filter
{
    public int Days { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }

    public override string FilterType
    {
        get
        {
            return "DaysChangeFilter"; // Return the string value, not the class name
        }
    }

    public override string GetDescription()
    {
        return string.Concat("近", Days, "天漲幅在", Min, "% ~ ", Max, "%之間");
    }

    public override List<IIdentifiable> Execute(List<TradingData> tradingDatas)
    {
        List<IIdentifiable> stocks = new List<IIdentifiable>();

        foreach (var group in tradingDatas.GroupBy(td => td.StockId))
        {
            var dataOrderByDate = group.OrderByDescending(r => r.Date);
            var first = dataOrderByDate.FirstOrDefault();
            var day = dataOrderByDate.ElementAtOrDefault(Days);
            var priceChangeRate = 100 * (first.ClosePrice - day.ClosePrice) / day.ClosePrice;

            if (priceChangeRate < Max && priceChangeRate > Min)
            {
                stocks.Add(first.Stock);
            }
        }

        return stocks;
    }
}