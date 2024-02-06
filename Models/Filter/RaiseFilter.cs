namespace azuretest.Models;

public class RiseFilter : ConsecutiveDaysFilter
{
    public override string FilterType => "RiseFilter";
    public override string GetDescription() => $"已連續 {Days} 天上漲";

    protected override bool PriceComparison(decimal previousPrice, decimal currentPrice)
    {
        return currentPrice < previousPrice;
    }
}