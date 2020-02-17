using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

using Banking.Core;
using Banking.Core.Models;
using Banking.Core.Helper;
using Banking.Core.Enums;



namespace Banking.Core.Test
{
    [TestClass]
    public class BankTransactionHelperTest
    {
        [TestMethod]
        public void BankTransaction_GetAllBankingTransactions()
        {
            int accountNumber = 123456;

            BankTransactionService bankHelper = new BankTransactionService();
            var transaction = bankHelper.GetAllBankingTransactions(accountNumber);

            Assert.IsNotNull(transaction);





        }

        [TestMethod]
        public void BankTransaction_AddTransaction()
        {
            //The account number we will be using. 
            int accountNumber = 123456;

            //The transaction 1 we will be using. 
            BankTransaction transaction1 = new BankTransaction();
            transaction1.AccountNumber = accountNumber;
            transaction1.PostedDate = DateTime.Now;
            transaction1.PostedAmount = 3000;
            transaction1.Description = "Initial Deposit";
            transaction1.TransactionType = BankingTransactionType.deposit;

            //The transaction we will be using. 
            BankTransaction transaction2 = new BankTransaction();
            transaction2.AccountNumber = accountNumber;
            transaction2.PostedDate = DateTime.Now;
            transaction2.PostedAmount = 15.95m;
            transaction2.Description = "Trader Joes";
            transaction2.TransactionType = BankingTransactionType.withdrawl;

            //The transaction 3 we will be using. 
            BankTransaction transaction3 = new BankTransaction();
            transaction3.AccountNumber = accountNumber;
            transaction3.PostedDate = DateTime.Now;
            transaction3.PostedAmount = 30.99m;
            transaction3.Description = "AMC Movie Theater";
            transaction3.TransactionType = BankingTransactionType.withdrawl;

            
            //Let's get the bank helper.
            BankTransactionService bankHelper = new BankTransactionService();
            
            //Let's add the transaction helper.
            var Transaction1Posted = bankHelper.AddTransaction(transaction1);

            //Do a check of all transactions and look for the ID that you added. - Transaction 1
            var transactionsAfterTransaction1Added = bankHelper.GetAllBankingTransactions(accountNumber);

            //Let's see if the transactionID was added to the cache. - - Transaction 1
            bool hasTransaction1 = transactionsAfterTransaction1Added.Any(transAdded => transAdded.BankTransactionID == Transaction1Posted.BankTransactionID);

            //Assert that we have added the record. - Transaction 1
            Assert.IsTrue(hasTransaction1);

            //-------------------------------------------------------------------------------------

            //Let's add the transaction helper.
            var Transaction2Posted = bankHelper.AddTransaction(transaction2);

            //Do a check of all transactions and look for the ID that you added. - Transaction 1
            var transactionsAfterTransaction2Added = bankHelper.GetAllBankingTransactions(accountNumber);

            //Let's see if the transactionID was added to the cache. - - Transaction 1
            bool hasTransaction2 = transactionsAfterTransaction2Added.Any(transAdded => transAdded.BankTransactionID == Transaction2Posted.BankTransactionID);

            //Assert that we have added the record. - Transaction 1
            Assert.IsTrue(hasTransaction2);

            //-------------------------------------------------------------------------------------

            //Let's add the transaction helper.
            var Transaction3Posted = bankHelper.AddTransaction(transaction3);

            //Do a check of all transactions and look for the ID that you added. - Transaction 1
            var transactionsAfterTransaction3Added = bankHelper.GetAllBankingTransactions(accountNumber);

            //Let's see if the transactionID was added to the cache. - - Transaction 1
            bool hasTransaction3 = transactionsAfterTransaction3Added.Any(transAdded => transAdded.BankTransactionID == Transaction3Posted.BankTransactionID);

            //Assert that we have added the record. - Transaction 1
            Assert.IsTrue(hasTransaction3);


        }


        [TestMethod]
        public void BankTransaction_GetBalance()
        {
            //The account number we will be using. 
            int accountNumber = 100974;

            //The transaction 1 we will be using. 
            BankTransaction transaction1 = new BankTransaction();
            transaction1.AccountNumber = accountNumber;
            transaction1.PostedDate = DateTime.Now;
            transaction1.PostedAmount = 3000;
            transaction1.Description = "Initial Deposit";
            transaction1.TransactionType = BankingTransactionType.deposit;

            //The transaction we will be using. 
            BankTransaction transaction2 = new BankTransaction();
            transaction2.AccountNumber = accountNumber;
            transaction2.PostedDate = DateTime.Now;
            transaction2.PostedAmount = 15.95m;
            transaction2.Description = "Trader Joes";
            transaction2.TransactionType = BankingTransactionType.withdrawl;

            //The transaction 3 we will be using. 
            BankTransaction transaction3 = new BankTransaction();
            transaction3.AccountNumber = accountNumber;
            transaction3.PostedDate = DateTime.Now;
            transaction3.PostedAmount = 30.99m;
            transaction3.Description = "AMC Movie Theater";
            transaction3.TransactionType = BankingTransactionType.withdrawl;


            //Let's get the bank helper.
            BankTransactionService bankHelper = new BankTransactionService();

            //Let's add the transaction helper.
            var Transaction1Posted = bankHelper.AddTransaction(transaction1);

            //Do a check of all transactions and look for the ID that you added. - Transaction 1
            var transactionsAfterTransaction1Added = bankHelper.GetAllBankingTransactions(accountNumber);

            //Let's see if the transactionID was added to the cache. - - Transaction 1
            bool hasTransaction1 = transactionsAfterTransaction1Added.Any(transAdded => transAdded.BankTransactionID == Transaction1Posted.BankTransactionID);

            //Assert that we have added the record. - Transaction 1
            Assert.IsTrue(hasTransaction1);

            //-------------------------------------------------------------------------------------

            //Let's add the transaction helper.
            var Transaction2Posted = bankHelper.AddTransaction(transaction2);

            //Do a check of all transactions and look for the ID that you added. - Transaction 1
            var transactionsAfterTransaction2Added = bankHelper.GetAllBankingTransactions(accountNumber);

            //Let's see if the transactionID was added to the cache. - - Transaction 1
            bool hasTransaction2 = transactionsAfterTransaction2Added.Any(transAdded => transAdded.BankTransactionID == Transaction2Posted.BankTransactionID);

            //Assert that we have added the record. - Transaction 1
            Assert.IsTrue(hasTransaction2);

            //-------------------------------------------------------------------------------------

            //Let's add the transaction helper.
            var Transaction3Posted = bankHelper.AddTransaction(transaction3);

            //Do a check of all transactions and look for the ID that you added. - Transaction 1
            var transactionsAfterTransaction3Added = bankHelper.GetAllBankingTransactions(accountNumber);

            //Let's see if the transactionID was added to the cache. - - Transaction 1
            bool hasTransaction3 = transactionsAfterTransaction3Added.Any(transAdded => transAdded.BankTransactionID == Transaction3Posted.BankTransactionID);

            //Assert that we have added the record. - Transaction 1
            Assert.IsTrue(hasTransaction3);
            
            //Now let's get the sum 
            decimal accountBalance = bankHelper.GetBalance(accountNumber);

            //Verify the amount. 
            decimal testAmount = (transaction1.PostedAmount - transaction2.PostedAmount - transaction3.PostedAmount);
              
            
            //if(accountBalance != testAmount )
            //{
            //    throw new Exception(string.Format("There was an issue because accountBalance = {0} did not equal test amount = {1}", accountBalance, testAmount));
            //}

            Assert.IsTrue(accountBalance == testAmount);


        }


    }
}
