using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;


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

        public async Task<IActionResult> SelectBankAccount()
        {
            var model = await GetProfileInformation();
            
            return View(model);
        }

        public async Task<IActionResult> BankTransactions(ProfileInformation profileInformation)
        {
            int accountNumber = profileInformation.BankAccountNumbers.FirstOrDefault();
            return await BankTransactions(accountNumber);
        }
        
            public async Task<IActionResult> BankTransactions(int accountNumber)
        {
            

            var model = await GetBankTransactionsAsync(accountNumber);
            var accountBalance = await GetCurrentBalance(accountNumber);
            
            ViewBag.AccountBalance = accountBalance;
            
            return View(model);
        }

        public ActionResult CreateTransaction()
        {
            return View();



        }

        private async Task<decimal> GetCurrentBalance(int accountNumber)
        {
            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = this._httpClientFactory.CreateClient("BankTransactions"))
            {
                var balanceResult = await client.PostAsJsonAsync("getbalance", accountNumber);



                if (balanceResult.IsSuccessStatusCode)
                {
                    // Read all of the response and deserialise it into an instace of
                    // WeatherForecast class
                    var content = await balanceResult.Content.ReadAsAsync<decimal>();
                    return content;
                }
                else
                {                    
                    //There was an issue 
                    return 0;
                }

            }

        }
        private async Task<BankTransaction[]> GetBankTransactionsAsync(int accountNumber)
        {
            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = this._httpClientFactory.CreateClient("BankTransactions"))
            {

                var bankTransResults = await client.PostAsJsonAsync("gettransactions", accountNumber);



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

        private async Task<ProfileInformation> GetProfileInformation()
        {          

            ///We have the username in our claims store 
            var identity = (ClaimsIdentity)this.HttpContext.User.Identity;

            //Look for it. 
            var claim =  identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            //Get the value. 
            string userName = claim.Value;


            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = this._httpClientFactory.CreateClient("Users"))
            {
                ///Make a post to the API to get the profile information. 
                var profileResult = await client.PostAsJsonAsync("getprofileinfobyusername", userName);
                
                //Check to see if it was a success. 
                if (profileResult.IsSuccessStatusCode)
                {
                    // Read all of the response and deserialise it into an instace of
                    // WeatherForecast class
                    var content = await profileResult.Content.ReadAsAsync<ProfileInformation>();
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