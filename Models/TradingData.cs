using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using azureTest.Models;

namespace azureTest.Models
{
    public interface IIdentifiable
    {
        int GetId();
        string GetIdentificationInfo();
    }

    public class TradingData : IIdentifiable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TradeId { get; set; }
        public DateTime Date { get; set; }
        public int StockId { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public long TradingVolume { get; set; }
        public long TradingMoney { get; set; }
        public decimal Spread { get; set; }
        public int TradingTurnover { get; set; }

        // Navigation property
        public Stock Stock { get; set; }

        public int GetId() => this.TradeId;
        public string GetIdentificationInfo() => $"TradeId: {this.TradeId}, Date: {this.Date}";
    }

}

