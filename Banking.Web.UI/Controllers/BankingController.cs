using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;

using Banking.Core.Models;

namespace Banking.Web.UI.Controllers
{
    public class BankingController : Controller
    {
        IHttpClientFactory _httpClientFactory;

        public BankingController(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }


        public async Task<IActionResult> BankTransactions()
        {
            var model = await GetBankTransactionsAsync(); 
            return View(model);
        }


        private async Task<BankTransaction[]> GetBankTransactionsAsync()
        {
            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = this._httpClientFactory.CreateClient("BankTransactions"))
            {

                var bankTransResults = await client.PostAsJsonAsync("gettransactions", 99868786);



                if (bankTransResults.IsSuccessStatusCode)
                {
                    // Read all of the response and deserialise it into an instace of
                    // WeatherForecast class
                    var content = await bankTransResults.Content.ReadAsAsync<BankTransaction[]>();
                    return content;
                }
                else
                {
                    //There was an issue 
                    return null;
                }

            }

            
        }

    }
}