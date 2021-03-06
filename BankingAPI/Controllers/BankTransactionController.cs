﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer; 



using Banking.Core.Interfaces;
using Banking.Core.Models;

namespace BankingAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class BankTransactionController : ControllerBase
    {
        private IBankTransactionService _bankTransactionService;

        public BankTransactionController(IBankTransactionService bankTransactionService)
        {
            _bankTransactionService = bankTransactionService;
        }


        /// <summary>
        /// This retrieves all of the bank transactions for specific account. 
        /// </summary>
        /// <param name="accountNumber">This is the account number that we are wishing to retrieve data for.</param>
        /// <returns>An action result if things went OK with an array of banking transactions.</returns>
        [HttpPost("gettransactions")]
        public IActionResult GetTransactions([FromBody]int accountNumber)
        {
            //Get the transactions.
            try
            {
                //Check to see if the account number exists. If not return 
                bool accountNumberExists = this._bankTransactionService.CheckforAccountNumber(accountNumber);
               
                if (!accountNumberExists)
                {
                    return NotFound();
                }

                var transactions = this._bankTransactionService.GetAllBankingTransactions(accountNumber).ToArray();
                
                return Ok(transactions);

            }
            catch (Exception ex)
            {
                //TODO: Need to look at the error and create more specific status codes for this. 
                //If there was an error send back the bad request.
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        /// <summary>
        /// This retrives the current balance. 
        /// </summary>
        /// <param name="accountNumber">This is the account number that we are wishing to retrieve data for.</param>
        /// <returns>An action result if things went OK with an int of the balance of the data.</returns>
        [HttpPost("getbalance")]
        public IActionResult GetBalance([FromBody]int accountNumber)
        {
            //Get the transactions.
            try
            {
                //Check to see if the account number exists. If not return 
                bool accountNumberExists = this._bankTransactionService.CheckforAccountNumber(accountNumber);
                if (!accountNumberExists)
                {
                    return NotFound();
                }


                var balance = this._bankTransactionService.GetBalance(accountNumber);

                return Ok(balance);

            }
            catch (Exception ex)
            {
                //TODO: Need to look at the error and create more specific status codes for this.
                //If there was an error send back the bad request.
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }


        

        
        /// <summary>
        /// This is the method for adding banktransactions to the system.
        /// </summary>
        /// <remarks>
        /// This will be for both the withdrawls and the deposits.
        /// </remarks>
        [HttpPost("posttransaction")]
        public IActionResult Post([FromBody]BankTransaction bankTransaction)
        {
           
            //Add the transaction.
            try
            {
               

                var transaction = this._bankTransactionService.AddTransaction(bankTransaction);

                if (transaction.BankTransactionID == Guid.Empty)
                {
                    //throw an exception and catch at top. 
                    throw new Exception("There was an error when attempting to add transaction the bank transaction has no BankTransactionID");
                }
                else
                {
                    //TODO: This needs to be changed to created with the path back to where it was created. Created(string, object)
                    //return that all is ok
                    return Ok();
                }
                                
            }
            catch(Exception ex)
            {
                //TODO: Need to look at the error and create more specific status codes for this.
                //If there was an error send back the bad request.
                return BadRequest(new { message = ex.Message.ToString() });

            }

        }


       





    }
}
