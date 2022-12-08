using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwelveDataDemo.Data;
using TwelveDataDemo.Data.Models;
using TwelveDataDemo.Models.RealTimePrice;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;

namespace TwelveDataDemo.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    public class TwelveDataController : Controller
    {
        private readonly TwelveDataContext dbContext;
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;

        public TwelveDataController(TwelveDataContext dbContext, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.clientFactory = clientFactory;
            this.configuration = configuration;
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RealTimePrice(string symbol)
        {
            var newPrice = await PostRealTimePrice("EUR/USD");
            return View(newPrice);
        }
               
        public async Task<TwelveDataPriceViewModel> PostRealTimePrice(string symbol)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"price?symbol={symbol}&apikey={this.configuration["TwelveData:ApiKey"]}&format=JSON");
            var client = this.clientFactory.CreateClient("twelveData");
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<TwelveDataPrice>(body);
                TwelveDataPriceViewModel result = new TwelveDataPriceViewModel
                {
                    Id = GenerateGuid(),
                    Symbol = symbol,
                    Price = jsonResponse.Price
                };
                //this.dbContext.TwelveDataPrices.Add(result);
                //await this.dbContext.SaveChangesAsync();

                return result;
                //return CreatedAtAction("GetRealTimePrice", new { id = result.Id }, result);
            }
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<TwelveDataPrice>> GetRealTimePrice(string id)
        //{
        //    var result = await dbContext
        //        .TwelveDataPrices
        //        .FindAsync(id);

        //    //if (result == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    return result;
        //}

        //public async Task<TwelveDataPrice> GetRealTimePriceAsync()
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, $"price?symbol=AAPL&apikey={this.configuration["TwelveData:ApiKey"]}&format=JSON");
        //    var client = this.clientFactory.CreateClient("twelveData");
        //    using (var response = await client.SendAsync(request))
        //    {
        //        response.EnsureSuccessStatusCode();
        //        var body = await response.Content.ReadAsStringAsync();
        //        var jsonResponse = JsonConvert.DeserializeObject<TwelveDataPriceResult>(body);
        //        TwelveDataPrice result = new TwelveDataPrice
        //        {
        //            Id = GenerateGuid(),
        //            Price = jsonResponse.Price
        //        };
        //        this.dbContext.TwelveDataPrices.Add(result);
        //        await this.dbContext.SaveChangesAsync();
        //        return result;

        //    }
        //}

        private string GenerateGuid()
        {
            Guid guid = Guid.NewGuid();
            string str = guid.ToString();
            return str;
        }
    }
}
