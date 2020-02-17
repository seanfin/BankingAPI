using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Caching;

using Banking.Core.Models;
using Banking.Core.Enums;
using Banking.Core.Interfaces;

namespace Banking.Core.Helper
{
    public class BankTransactionService: IBankTransactionService
    {
        //TODO: move this to the appsettings.
        //Set the cache to expire in 30 minutes.
        private readonly int DEFAULT_CACHE_EXPIRATION_MINUTES = 30;


        /// <summary>
        /// Adds a transaction to DB/Cache. 
        /// </summary>
        /// <param name="bankTransaction">The Model/Object representing the bank transaction that we wish to add/insert into the DB/cache.</param>
        public BankTransaction AddTransaction (BankTransaction bankTransaction)
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


            try
            {
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
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(DEFAULT_CACHE_EXPIRATION_MINUTES)
                };

                //let's create a cache item. 
                var itemToCache = new CacheItem(bankTransaction.AccountNumber.ToString(), transactions); 
                
                //Now lets set the cache container to have the new data. 
                cacheContainer.Set(itemToCache, policy);


            }
            catch
            ( Exception ex)
            {
                throw ex;
            }

            return bankTransaction;
        }

        /// <summary>
        /// Get all BankingTransactions
        /// </summary>
        /// <param name="accountNumber">The bank number associated with the banking transactions that are being returned. </param>
        public List<BankTransaction> GetAllBankingTransactions(int accountNumber )
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

      






    }
}
