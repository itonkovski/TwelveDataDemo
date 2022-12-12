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
using System.Collections;
using System.Collections.Generic;

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

        public IActionResult RealTimePrice()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RealTimePrice(string symbol)
        {
            var newPrice = await PostRealTimePrice(symbol);
            //return View(newPrice);
            return RedirectToAction(nameof(AllPrices));
        }

        public IActionResult AllPrices()
        {
            var prices = GetPrice();
            return View(prices);
        }

        //public async Task GetJsonResponse(TwelveDataPriceFormModel priceModel, string symbol)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, $"price?symbol={symbol}&apikey={this.configuration["TwelveData:ApiKey"]}&format=JSON");
        //    var client = this.clientFactory.CreateClient("twelveData");
        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();
        //    var body = await response.Content.ReadAsStringAsync();
        //    var jsonResponse = JsonConvert.DeserializeObject<TwelveDataPrice>(body);
        //    await AddPriceAsync(priceModel);
        //}

        //public async Task AddPriceAsync(TwelveDataPriceFormModel priceModel)
        //{
        //    var newPrice = new TwelveDataPrice
        //    {
        //        Id = priceModel.Id,
        //        Symbol = priceModel.Symbol,
        //        Price = priceModel.Price
        //    };
        //    await this.dbContext.TwelveDataPrices.AddAsync(newPrice);
        //    await this.dbContext.SaveChangesAsync();

        //}

        private async Task<TwelveDataPrice> PostRealTimePrice(string symbol)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"price?symbol={symbol}&apikey={this.configuration["TwelveData:ApiKey"]}&format=JSON");
            var client = this.clientFactory.CreateClient("twelveData");
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<TwelveDataPriceFormModel>(body);
                var result = new TwelveDataPrice
                {
                    Id = GenerateGuid(),
                    Symbol = symbol,
                    Price = jsonResponse.Price
                };
                this.dbContext.TwelveDataPrices.Add(result);
                await this.dbContext.SaveChangesAsync();

                return result;
            }
        }

        private IEnumerable<TwelveDataPriceViewModel> GetPrice()
        {
            var price = this.dbContext
                .TwelveDataPrices
                .Select(x => new TwelveDataPriceViewModel
                {
                    Id = x.Id,
                    Symbol = x.Symbol,
                    Price = x.Price
                })
                .ToList();
            return price;
        }

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
