using System;
using System.ComponentModel.DataAnnotations;

namespace TwelveDataDemo.Models.RealTimePrice
{
    public class TwelveDataPriceViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Symbol { get; set; }

        public string Price { get; set; }
    }
}
