using System;
using Microsoft.EntityFrameworkCore;
using TwelveDataDemo.Data.Models;

namespace TwelveDataDemo.Data
{
    public class TwelveDataContext : DbContext
    {
        public TwelveDataContext(DbContextOptions<TwelveDataContext>options)
            :base(options)
        {

        }

        public DbSet<TwelveDataPrice> TwelveDataPrices { get; set; }
    }
}
