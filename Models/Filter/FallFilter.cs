namespace azuretest.Models;

public class FallFilter : ConsecutiveDaysFilter
{
    public override string FilterType => "FallFilter";
    public override string GetDescription() => $"已連續 {Days} 天下跌";

    protected override bool PriceComparison(decimal previousPrice, decimal currentPrice)
    {
        return currentPrice > previousPrice;
    }
}