namespace azuretest.Models;

public abstract class Filter
{
    public virtual string FilterType { get; set; }
    public abstract string GetDescription();
    public abstract List<IIdentifiable> Execute(List<TradingData> tradingDatas);

    public int? result { get; set; }
}

public enum ComparisonOperators
{
    GreaterThan,  // >
    LessThan,     // <
    GreaterThanOrEqualTo, // >=
    LessThanOrEqualTo,   // <=
    EqualTo  // =
}

public enum FilterStrategy
{
    StockPrice,
    RecentDaysRise,
    RecentDaysFall,
    recentDaysChange,
}

public static class ComparisonOperatorsExtensions
{
    public static string ToSymbol(this ComparisonOperators op)
    {
        switch (op)
        {
            case ComparisonOperators.GreaterThan:
                return ">";
            case ComparisonOperators.LessThan:
                return "<";
            case ComparisonOperators.GreaterThanOrEqualTo:
                return ">=";
            case ComparisonOperators.LessThanOrEqualTo:
                return "<=";
            case ComparisonOperators.EqualTo:
                return "=";
            default:
                throw new ArgumentOutOfRangeException(nameof(op), op, null);
        }
    }
}

public static class DataStorage
{
    public static List<Filter> Filters { get; set; } = new List<Filter>();
}