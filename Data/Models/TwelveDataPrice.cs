using System;
using System.ComponentModel.DataAnnotations;
using TwelveDataDemo.Data.Models.Enums;

namespace TwelveDataDemo.Data.Models
{
    public class TwelveDataPrice
    {
        public string Id { get; set; }

        [Required]
        public string Symbol { get; set; }

        public string Price { get; set; }
    }
}
