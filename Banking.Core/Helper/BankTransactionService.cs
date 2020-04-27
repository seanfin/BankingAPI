using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Caching;
using Microsoft.Extensions.Options;

using Banking.Core.Models;
using Banking.Core.Enums;
using Banking.Core.Interfaces;
using Banking.Core.Utils;
using System.Linq;

namespace Banking.Core.Helper
{
    public class BankTransactionService : IBankTransactionService
    {
        //TODO: move this to the appsettings.
        //Set the cache to expire in 30 minutes.


        private readonly AppSettings _appSettings;

        public BankTransactionService(IOptions<AppSettings> appSettings)
        {
            this._appSettings = appSettings.Value;

            //Let's create some initial accounts to start off with for display purposes. 
            int accountNumber1 = 99868786;
            AddGhostTransactions1(accountNumber1);

            //Let's create some initial accounts to start off with for display purposes. 
            int accountNumber2 = 584752341;
            AddGhostTransactions2(accountNumber2);

            //Let's create some initial accounts to start off with for display purposes. 
            int accountNumber3 = 897562213;
            AddGhostTransactions1(accountNumber3);

            //Let's create some initial accounts to start off with for display purposes. 
            int accountNumber4 = 1847562247;
            AddGhostTransactions2(accountNumber4);





        }

        /// <summary>
        /// Adds a transaction to DB/Cache. 
        /// </summary>
        /// <param name="bankTransaction">The Model/Object representing the bank transaction that we wish to add/insert into the DB/cache.</param>
        public BankTransaction AddTransaction(BankTransaction bankTransaction)
        {
            //Check to see if the account number is populated if not we need to throw an exception. 
            if (bankTransaction.AccountNumber == -1)
            {
                throw new Exception("In order to add a Transaction the bankTransaction object needs to have an account number associated with it.");
            }
            else if (bankTransaction.PostedAmount == 0)
            {
                throw new Exception("Please attach an a posted amount when created a transaction.");

            }



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Special note: I am unfamiliar with how the banking industry works, so they could very well be refunding on a withdrawl 
            //here and we could not want to use Math.Abs. However, this would be where I would want to speak to the client and have 
            //some more industry information. 


            //Additionally let's just make sure that they are using absolute values when they are adding the amounts. 
            bankTransaction.PostedAmount = Math.Abs(bankTransaction.PostedAmount);

            //Let's get the transactions in the database.
            List<BankTransaction> transactions = GetAllBankingTransactions(bankTransaction.AccountNumber);


            //We add the guid to the transaction before we add it. 
            bankTransaction.BankTransactionID = Guid.NewGuid();

            //Let's add the transaction to the list. 
            transactions.Add(bankTransaction);


            //Get a reference to the default MemoryCache instance.
            var cacheContainer = MemoryCache.Default;

            //Create a cache policy so that it will expire eventually.
            var policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(this._appSettings.CacheExpirationInMinutes)
            };

            //let's create a cache item. 
            var itemToCache = new CacheItem(bankTransaction.AccountNumber.ToString(), transactions);

            //Now lets set the cache container to have the new data. 
            cacheContainer.Set(itemToCache, policy);




            return bankTransaction;
        }

        /// <summary>
        /// Get all BankingTransactions
        /// </summary>
        /// <param name="accountNumber">The bank number associated with the banking transactions that are being returned. </param>
        public List<BankTransaction> GetAllBankingTransactions(int accountNumber)
        {



            //Let's create an object for populating with the transactions. 
            List<BankTransaction> transactions = new List<BankTransaction>();

            //Check to see if the account number is populated if not we need to throw an exception. 
            if (accountNumber == -1)
            {
                throw new Exception("In order to get all Transactions the method needs to have an account number.");
            }

            try
            {

                lock (this)
                {


                    //Get a reference to the default MemoryCache instance.
                    var cacheContainer = MemoryCache.Default;

                    //Let's return the transactions with the account. 
                    var existingBankTrans = cacheContainer.Get(accountNumber.ToString());

                    //If the resule are not null then let's try and populate the transaction. 
                    if (existingBankTrans != null)
                    {
                        //Load up the transactions into the List. 
                        transactions = (List<BankTransaction>)existingBankTrans;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return transactions;

        }

        /// <summary>
        /// Get all BankingTransactions
        /// </summary>
        /// <param name="accountNumber">The bank number associated with the banking transactions that are being returned. </param>
        public bool CheckforAccountNumber(int accountNumber)
        {
            //Check to see if the account number is populated if not we need to throw an exception. 
            if (accountNumber == -1)
            {
                throw new Exception("In order to get all Transactions the method needs to have an account number.");
            }

            var accountNumberExists = false;

                lock (this)
                {


                    //Get a reference to the default MemoryCache instance.
                    var cacheContainer = MemoryCache.Default;

                //Let's return the transactions with the account. 
                accountNumberExists = cacheContainer.Contains(accountNumber.ToString());

                }

            return accountNumberExists;
        }



        /// <summary>
        /// Retrieves the balance from the transactions. 
        /// </summary>
        /// <param name="accountNumber">Simply needs the account number</param>
        /// <returns>A decimal number with the account balance.</returns>
        public decimal GetBalance(int accountNumber)
        {
            decimal accountBalance = 0;

            //Let's get the transactions in the database.
            List<BankTransaction> transactions = GetAllBankingTransactions(accountNumber);

            //Let's loop through and get the data. 
            foreach (BankTransaction transaction in transactions)
            {

                //don't waste time on the zero amount for just getting the amount.
                if (transaction.PostedAmount == 0)
                {
                    continue;
                }
                //Let's add the deposit.
                else if (transaction.TransactionType == BankingTransactionType.deposit)
                {
                    accountBalance += transaction.PostedAmount;
                }
                //Let's subtract the withdrawl.
                else if (transaction.TransactionType == BankingTransactionType.withdrawl)
                {
                    accountBalance -= transaction.PostedAmount;
                }
            }

            return accountBalance;
        }

        
        private void AddGhostTransactions1(int accountNumber)
        {
            //The transaction 1 we will be using. 
            BankTransaction transaction1 = new BankTransaction();
            transaction1.AccountNumber = accountNumber;
            transaction1.PostedDate = DateTime.Now;
            transaction1.PostedAmount = 3000;
            transaction1.Description = "Initial Deposit";
            transaction1.TransactionType = BankingTransactionType.deposit;

            AddTransaction(transaction1);


            //The transaction we will be using. 
            BankTransaction transaction2 = new BankTransaction();
            transaction2.AccountNumber = accountNumber;
            transaction2.PostedDate = DateTime.Now;
            transaction2.PostedAmount = 15.95m;
            transaction2.Description = "Trader Joes";
            transaction2.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction2);

            //The transaction 3 we will be using. 
            BankTransaction transaction3 = new BankTransaction();
            transaction3.AccountNumber = accountNumber;
            transaction3.PostedDate = DateTime.Now;
            transaction3.PostedAmount = 30.99m;
            transaction3.Description = "AMC Movie Theater";
            transaction3.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction3);


            //The transaction 4 we will be using. 
            BankTransaction transaction4 = new BankTransaction();
            transaction4.AccountNumber = accountNumber;
            transaction4.PostedDate = DateTime.Now;
            transaction4.PostedAmount = 22.98m;
            transaction4.Description = "Fred Meyer";
            transaction4.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction4);


            //The transaction 5 we will be using. 
            BankTransaction transaction5 = new BankTransaction();
            transaction5.AccountNumber = accountNumber;
            transaction5.PostedDate = DateTime.Now;
            transaction5.PostedAmount = 16.95m;
            transaction5.Description = "NetFlix";
            transaction5.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction5);

            //The transaction 6 we will be using. 
            BankTransaction transaction6 = new BankTransaction();
            transaction6.AccountNumber = accountNumber;
            transaction6.PostedDate = DateTime.Now;
            transaction6.PostedAmount = 30.00m;
            transaction6.Description = "Starbucks Card Reload";
            transaction6.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction6);


        }


        private void AddGhostTransactions2(int accountNumber)
        {
            //The transaction 1 we will be using. 
            BankTransaction transaction1 = new BankTransaction();
            transaction1.AccountNumber = accountNumber;
            transaction1.PostedDate = DateTime.Now;
            transaction1.PostedAmount = 3000;
            transaction1.Description = "Initial Deposit";
            transaction1.TransactionType = BankingTransactionType.deposit;

            AddTransaction(transaction1);


            //The transaction we will be using. 
            BankTransaction transaction2 = new BankTransaction();
            transaction2.AccountNumber = accountNumber;
            transaction2.PostedDate = DateTime.Now;
            transaction2.PostedAmount = 15.95m;
            transaction2.Description = "Trader Joes";
            transaction2.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction2);

            //The transaction 3 we will be using. 
            BankTransaction transaction3 = new BankTransaction();
            transaction3.AccountNumber = accountNumber;
            transaction3.PostedDate = DateTime.Now;
            transaction3.PostedAmount = 30.99m;
            transaction3.Description = "AMC Movie Theater";
            transaction3.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction3);

            //The transaction 4 we will be using. 
            BankTransaction transaction4 = new BankTransaction();
            transaction4.AccountNumber = accountNumber;
            transaction4.PostedDate = DateTime.Now;
            transaction4.PostedAmount = 78.95m;
            transaction4.Description = "Fred Meyer";
            transaction4.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction4);


            //The transaction 5 we will be using. 
            BankTransaction transaction5 = new BankTransaction();
            transaction5.AccountNumber = accountNumber;
            transaction5.PostedDate = DateTime.Now;
            transaction5.PostedAmount = 100.00m;
            transaction5.Description = "Amazon";
            transaction5.TransactionType = BankingTransactionType.withdrawl;

            AddTransaction(transaction5);

            //The transaction 6 we will be using. 
            BankTransaction transaction6 = new BankTransaction();
            transaction6.AccountNumber = accountNumber;
            transaction6.PostedDate = DateTime.Now;
            transaction6.PostedAmount = 2000.00m;
            transaction6.Description = "Ach Paycheck";
            transaction6.TransactionType = BankingTransactionType.deposit;

            AddTransaction(transaction6);


        }










    }
}
