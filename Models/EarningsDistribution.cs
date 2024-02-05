using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using azureTest.Models;

namespace azureTest.Models
{
    public class EarningsDistribution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistributionId { get; set; }
        public DateTime Date { get; set; }
        public int StockId { get; set; }
        public decimal CashEarningsDistribution { get; set; }

        // Navigation property
        public Stock Stock { get; set; }
    }

}

