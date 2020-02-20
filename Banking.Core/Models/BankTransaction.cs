using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

using Banking.Core.Enums;

namespace Banking.Core.Models
{
    public class BankTransaction
    {
        public Guid BankTransactionID { get; set; }

        [Required]
        public int AccountNumber { get; set; }

        [Required]
        public DateTime PostedDate { get; set; }

        [Required]
        public decimal PostedAmount { get; set; }

        public string Description { get; set; }

        [Required]
        public BankingTransactionType TransactionType {get; set;}

        

    }
}
