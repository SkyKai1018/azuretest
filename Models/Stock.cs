using System.ComponentModel.DataAnnotations;

namespace azuretest.Models;

public class Stock : IIdentifiable
{
    [Key]
    public int StockId { get; set; }
    public string StockName { get; set; }

    // Navigation properties
    public ICollection<TradingData> TradingDatas { get; set; }
    public ICollection<EarningsDistribution> EarningsDistributions { get; set; }

    public int GetId() => this.StockId;
    public string GetIdentificationInfo() => $"Stock: {this.StockName}";
}

