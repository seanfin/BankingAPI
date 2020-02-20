using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using PanoramicData.ConsoleExtensions;

using Banking.Core.Models;
using Banking.Core.Utils;
using Banking.Core.Enums;

namespace BankingConsole
{

    

    public class BankingConsoleActions
    {

        public static readonly string Command_Login = "Login";
        public static readonly string Command_CreateAccount = "CreateAccount";
        public static readonly string Command_PostTransaction = "PostTransaction";
        public static readonly string Command_GetAccountBalance = "GetAccountBalance";
        public static readonly string Command_GetTransactionHistory = "GetTransactionHIstory";
        public static readonly string Command_Logout = "LogOut";
        public static readonly string Command_Helper = "Helper";


        private readonly string DICATIONARYKEY_TOKEN = "KEYTOKEN";
        private readonly string DICATIONARYKEY_LOGIN = "KEYLOGIN";

        ExternalAppSettings _externalAppSettings;
        Dictionary<string, string> _dictionaryStore = new Dictionary<string, string>(); 
        

        public BankingConsoleActions()
        {
            this._externalAppSettings = AppSettingHelper.GetExternalApplicationConfiguration();

        }

        /// <summary>
        /// Attempts to create a new account. 
        /// </summary>
        public void CreateNewAccount()
        {

            Console.Write("Register New Account");
            Console.Write("What would you like your username to be? Please use email address.");
            var userName = Console.ReadLine();
            Console.Write("What would you like your password to be?");
            var password = Console.ReadLine();

            AuthenticateModel authenticateModel = new AuthenticateModel();
            authenticateModel.Username = userName;
            authenticateModel.Password = password;
            authenticateModel.Role = Role.User;



            using (var client = new HttpClient())
            {
                //Getting access to our web api. 
                client.BaseAddress = new Uri(this._externalAppSettings.WebApiURLUser);

                //Putting our username and password in a model because I want them going over in the body. 


                //Put them over in a post because I feel it works better than a get method. 
                var responseTask = client.PostAsJsonAsync("createauthenicationaccount", authenticateModel);
                responseTask.Wait();


                //Let's look at the response. 
                var authResult = responseTask.Result;

                if (!authResult.IsSuccessStatusCode)
                {

                    throw new Exception("There was an issue while creating your account");
                }

                Console.WriteLine("Successfully created your account!");


            }

        }

        /// <summary>
        /// Attempts to login. 
        /// </summary>
        public async void Login()
        {
            Console.WriteLine("Login");
            Console.WriteLine("Please provider your username:");
            var userName = Console.ReadLine();
            Console.WriteLine("Please provider your password:");
            var password = ConsolePlus.ReadPassword();


            //Putting our username and password in a model because I want them going over in the body. 
            AuthenticateModel authenticateModel = new AuthenticateModel();
            authenticateModel.Username = userName;
            authenticateModel.Password = password;


            using (var client = new HttpClient())
            {
                //Getting access to our web api. 
                client.BaseAddress = new Uri(this._externalAppSettings.WebApiURLUser);
                                

                //Put them over in a post because I feel it works better than a get method. 
                var responseTask = client.PostAsJsonAsync("authenticate", authenticateModel);
                responseTask.Wait();


                //Let's look at the response. 
                var authResult = responseTask.Result;

                if (!authResult.IsSuccessStatusCode)
                {

                    throw new Exception("There was an issue while creating your account");
                }
                else 
                {
                    //Let's get the authentication model with the information populated. 
                    var userLogin = await authResult.Content.ReadAsAsync<AuthenticateModel>();

                    //Clean out all duplicate values
                    this._dictionaryStore.Clear();
                    
                    //We need to store some information in our dictionary. 
                    this._dictionaryStore.Add(DICATIONARYKEY_TOKEN, userLogin.Token);

                    //Need to add the login to the dictionary. 
                    this._dictionaryStore.Add(DICATIONARYKEY_LOGIN, userLogin.Username);

                    Console.WriteLine("Successful Login!");
                    
                                                         
                }
                                               
            }




        }

        public async void PostTransaction()
        {
            if (this._dictionaryStore[DICATIONARYKEY_TOKEN] == null)
                throw new Exception("You do not have a token please sign in.");
            if (this._dictionaryStore[DICATIONARYKEY_LOGIN] == null)
                throw new Exception("You do not have a login please sign in.");


            //Put together the bank transaction with the user. 
            Console.WriteLine("Bank Transaction");

            //Get the account number.
            Console.WriteLine("Please provide the account number");
            var accountNumberStr = Console.ReadLine();

            while(!Int32.TryParse(accountNumberStr.Trim(), out int accountNumber))
            {
                Console.WriteLine("The value you entered was not numeric.");
                accountNumberStr = Console.ReadLine();
            }

            //Get the PostedAmount
            Console.WriteLine("How much is this for?");
            var postedAmountStr = Console.ReadLine();

            while (!decimal.TryParse(postedAmountStr.Trim(), out decimal amount))
            {
                Console.WriteLine("The value you entered was not numeric.");
                postedAmountStr = Console.ReadLine();
            }

            var postedAmount = Convert.ToDecimal(postedAmountStr);


            //What is the description? 
            
            Console.WriteLine("What is the description for this transaction?");
            var description = Console.ReadLine();

            //Get the type of withdrawl 

            BankingTransactionType transType = BankingTransactionType.none;
            if(Math.Sign(postedAmount) > 0)
            {
                transType = BankingTransactionType.deposit;
            }
            else if(Math.Sign(postedAmount) < 0 )
            {
                transType = BankingTransactionType.withdrawl;
            }


            BankTransaction bankTransaction = new BankTransaction();
            bankTransaction.AccountNumber = Convert.ToInt32(accountNumberStr);
            bankTransaction.PostedAmount = postedAmount;
            bankTransaction.Description = description;
            bankTransaction.PostedDate = DateTime.Now;
            bankTransaction.TransactionType = transType;

            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = new HttpClient())
            {
                //Getting access to our web api. 
                client.BaseAddress = new Uri(this._externalAppSettings.WebApiURLBankTransaction);


                //Get the security Token. 
                string securityToken = this._dictionaryStore[DICATIONARYKEY_TOKEN];

                //Put together the header. 
                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", securityToken);

                //Add the header to the request. 
                client.DefaultRequestHeaders.Authorization = authHeaders;

                ///Make a post to the API to get the profile information. 
                var profileResult = await client.PostAsJsonAsync("posttransaction", bankTransaction);

                //Check to see if it was a success. 
                if (!profileResult.IsSuccessStatusCode)
                {
                    //There was an issue 
                    throw new Exception("Unable to Post your bank transaction.");

                }
                else
                {
                    // Read all of the response and deserialise it into an instace of
                    // WeatherForecast class
                    var content = await profileResult.Content.ReadAsAsync<BankTransaction>();

                    Console.WriteLine("Successfully posted your transaction!");

                }

            }



        }

        public async void GetCurrentBalance()
        {
            if (this._dictionaryStore[DICATIONARYKEY_TOKEN] == null)
                throw new Exception("You do not have a token please sign in.");
            if (this._dictionaryStore[DICATIONARYKEY_LOGIN] == null)
                throw new Exception("You do not have a login please sign in.");


            //Put together the bank transaction with the user. 
            Console.WriteLine("Bank Balance");

            //Please provide the account number you 
            Console.WriteLine("Please provide the account number");
            var accountNumberStr = Console.ReadLine();

            while (!Int32.TryParse(accountNumberStr.Trim(), out int accountNum))
            {
                Console.WriteLine("The value you entered was not numeric.");
                accountNumberStr = Console.ReadLine();
            }

            //convert to int 
            var accountNumber = Convert.ToInt32(accountNumberStr);


            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = new HttpClient())
            {

                //Getting access to our web api. 
                client.BaseAddress = new Uri(this._externalAppSettings.WebApiURLBankTransaction);

                //Get the security Token. 
                string securityToken = this._dictionaryStore[DICATIONARYKEY_TOKEN];

                //Put together the header. 
                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", securityToken);

                //Add the header to the request. 
                client.DefaultRequestHeaders.Authorization = authHeaders;

                //Let's get the balance. 
                var balanceResult = await client.PostAsJsonAsync("getbalance", accountNumber);

                //If there is a success let's type the data for return. 
                if (!balanceResult.IsSuccessStatusCode)
                {
                    throw new Exception("There was an issue when attempting to get the balance.");

                }
                else
                { 
                    //read into the variable.
                    var currentBalance = await balanceResult.Content.ReadAsAsync<decimal>();
                    
                    //Display the balance.
                    var maskedaccountNumber = SecurityHelper.GetMaskedAccountNumber(accountNumber);
                    var accountBalanceMessage = string.Format("The Current balancec for account ending in {0} = {1}", maskedaccountNumber, currentBalance);
                    Console.WriteLine(accountBalanceMessage);
                }
               

            }

        }


        /// <summary>
        /// Provides the transaction history on the account. 
        /// </summary>
        public async void GetTransactionHistory()
        {
            if (this._dictionaryStore[DICATIONARYKEY_TOKEN] == null)
                throw new Exception("You do not have a token please sign in.");
            if (this._dictionaryStore[DICATIONARYKEY_LOGIN] == null)
                throw new Exception("You do not have a login please sign in.");


            //Put together the bank transaction with the user. 
            Console.WriteLine("Transaction History");

            //Please provide the account number you 
            Console.WriteLine("Please provide the account number");
            var accountNumberStr = Console.ReadLine();

            while (!Int32.TryParse(accountNumberStr.Trim(), out int accountNum))
            {
                Console.WriteLine("The value you entered was not numeric.");
                accountNumberStr = Console.ReadLine();
            }

            //convert to int 
            var accountNumber = Convert.ToInt32(accountNumberStr);


            // Get an instance of HttpClient from the factpry that we registered
            // in Startup.cs
            using (var client = new HttpClient())
            {

                //Getting access to our web api. 
                client.BaseAddress = new Uri(this._externalAppSettings.WebApiURLBankTransaction);

                //Get the security Token. 
                string securityToken = this._dictionaryStore[DICATIONARYKEY_TOKEN];

                //Put together the header. 
                AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", securityToken);

                //Add the header to the request. 
                client.DefaultRequestHeaders.Authorization = authHeaders;

                //Post.
                var bankTransResults = await client.PostAsJsonAsync("gettransactions", accountNumber);

                //If we get a success then let's process the results.
                if (!bankTransResults.IsSuccessStatusCode)
                {

                    throw new Exception("There was an issue when attempting to pull your transactions.");
                }
                else
                {

                    //Lets read the results and make them into a type we can use. 
                    var content = await bankTransResults.Content.ReadAsAsync<BankTransaction[]>();


                    var headerLine = string.Format("PostedDate | Posted Amount | Transaction Type | Description");
                    Console.WriteLine(headerLine);

                    foreach (BankTransaction transaction in content)
                    {
                        var detail = string.Format("{0} | {1} | {2} | {3} ", transaction.PostedDate, transaction.PostedAmount, transaction.TransactionType, transaction.Description);
                        Console.WriteLine(detail);
                    }
                    
                }


            }

        }


        public bool LogOut()
        {
            //Clean out the values
            this._dictionaryStore.Clear();

            //Send back a message to turn off the app. 
            return true;


        }

        public void DisplayHelper()
        {

            Console.WriteLine("Banking Console Helper!");
            Console.WriteLine(string.Format("{0} = Will display the commands for the application", BankingConsoleActions.Command_Helper));
            Console.WriteLine(string.Format("{0} = Will log you into the system.",BankingConsoleActions.Command_Login));
            Console.WriteLine(string.Format("{0} = Will create assist you in creating an account.", BankingConsoleActions.Command_CreateAccount));
            Console.WriteLine(string.Format("{0} = Will post a transaction for you.", BankingConsoleActions.Command_PostTransaction));
            Console.WriteLine(string.Format("{0} = Will get an account balance for the account you select.", BankingConsoleActions.Command_GetAccountBalance));
            Console.WriteLine(string.Format("{0} = Will get transaction history for the account you select.", BankingConsoleActions.Command_GetTransactionHistory));
            Console.WriteLine(string.Format("{0} = Will log you out of the system and exit the program.", BankingConsoleActions.Command_Logout));
            


         





        }


            


        }
}
