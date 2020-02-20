using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

using Banking.Core.Utils;


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



        [HttpGet]
        public IActionResult CreateTransaction()
        {
            //Get the account number that should have been passed along. 
            int accountNumber = Convert.ToInt32(TempData["accountNumber"]);

            BankTransaction bankTransaction = new BankTransaction();
            bankTransaction.AccountNumber = accountNumber;
            bankTransaction.PostedDate = DateTime.Now;
            



            return View(bankTransaction);

        }


        [HttpPost]
        public async Task<IActionResult> CreateTransaction(BankTransaction bankTransaction)
        {

            var trans = await PostBankTransaction(bankTransaction);

            TempData["accountNumber"] = bankTransaction.AccountNumber;
            return RedirectToAction("BankTransactions", "Banking");
                        
        }


        [HttpPost]  
        public async Task<IActionResult> BankTransactions(ProfileInformation profileInformation)
        {
            int accountNumber = profileInformation.BankAccountNumbers.FirstOrDefault();
            TempData["accountNumber"] = accountNumber;

            return await BankTransactions();

        }

        [HttpGet]
        public async Task<IActionResult> BankTransactions()
        {
            //Get the account number that should have been passed along. 
            int accountNumber = Convert.ToInt32(TempData["accountNumber"]);

            var model = await GetBankTransactionsAsync(accountNumber);
            var accountBalance = await GetCurrentBalance(accountNumber);

            //Get a Masked AccountNumber. 
            var maskedAccountNumber = SecurityHelper.GetMaskedAccountNumber(accountNumber);

            ViewBag.AccountBalance = accountBalance;
            ViewBag.MaskedAccountNumber = maskedAccountNumber;

            //have to put this value back in here in case we are posting transactions. 
            TempData["accountNumber"] = accountNumber;

            return View(model);


        }


            public async Task<IActionResult> SearchForAccountByAccountNumber(BankTransaction bankTransaction)
            {

            if (bankTransaction.AccountNumber == 0)
                return View();


            //lets get the account number 
            int accountNumber = bankTransaction.AccountNumber;

            var result = await GetBankTransactionsAsync(accountNumber);

            if (result.Count() > 0)
            {
                TempData["accountNumber"] = accountNumber;
                return RedirectToAction("BankTransactions", "Banking");
            }
            else
            {
                ViewBag.UserMessages = "We could not find the account";
                return View();
            }

        }


        private async Task<decimal> GetCurrentBalance(int accountNumber)
        {
            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = this._httpClientFactory.CreateClient("BankTransactions"))
            {

                //Get the security Token. 
                string securityToken = GetSecurityToken();

                //Put together the header. 
                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", securityToken);

                //Add the header to the request. 
                client.DefaultRequestHeaders.Authorization = authHeaders;

                //Let's get the balance. 
                var balanceResult = await client.PostAsJsonAsync("getbalance", accountNumber);
                
                //If there is a success let's type the data for return. 
                if (balanceResult.IsSuccessStatusCode)
                {
                    //read into the variable.
                    var content = await balanceResult.Content.ReadAsAsync<decimal>();
                    return content;
                }
                else
                {
                    throw new Exception("There was an issue when attempting to get the balance.");
                }

            }

        }
        private async Task<BankTransaction[]> GetBankTransactionsAsync(int accountNumber)
        {
            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = this._httpClientFactory.CreateClient("BankTransactions"))
            {

                //Get the security Token. 
                string securityToken = GetSecurityToken();

                //Put together the header. 
                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", securityToken);
                
                //Add the header to the request. 
                client.DefaultRequestHeaders.Authorization = authHeaders; 

                //Post.
                var bankTransResults = await client.PostAsJsonAsync("gettransactions", accountNumber);

                //If we get a success then let's process the results.
                if (bankTransResults.IsSuccessStatusCode)
                {
                    //Lets read the results and make them into a type we can use. 
                    var content = await bankTransResults.Content.ReadAsAsync<BankTransaction[]>();
                    return content;
                }
                else
                {
                    throw new Exception("There was an issue when attempting to pull your transactions.");
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
            using (var client = this._httpClientFactory.CreateClient("Profiles"))
            {

                //Get the security Token. 
                string securityToken = GetSecurityToken();

                //Put together the header. 
                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", securityToken);

                //Add the header to the request. 
                client.DefaultRequestHeaders.Authorization = authHeaders;

                
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
                    throw new Exception("There was an issue attempting to get the profile of the current user.");
                }




            }

        }


        private async Task<BankTransaction> PostBankTransaction(BankTransaction bankTransaction)
        {            
            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = this._httpClientFactory.CreateClient("BankTransactions"))
            {

                //Get the security Token. 
                string securityToken = GetSecurityToken();

                //Put together the header. 
                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", securityToken);

                //Add the header to the request. 
                client.DefaultRequestHeaders.Authorization = authHeaders;

                ///Make a post to the API to get the profile information. 
                var profileResult = await client.PostAsJsonAsync("posttransaction", bankTransaction);

                //Check to see if it was a success. 
                if (profileResult.IsSuccessStatusCode)
                {
                    // Read all of the response and deserialise it into an instace of
                    // WeatherForecast class
                    var content = await profileResult.Content.ReadAsAsync<BankTransaction>();
                    return content;
                }
                else
                {
                    //There was an issue 
                    throw new Exception("Unable to Post your bank transaction.");
                }

            }

        }


        private string GetSecurityToken()
        {

            ///We have the username in our claims store 
            var identity = (ClaimsIdentity)this.HttpContext.User.Identity;

            //Look for it. 
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Hash);

            //Get the hash
            var securityToken = claim.Value;
            return securityToken;

        }







    }
}