using System;
using System.Collections.Generic;
using System.Text;

using Banking.Core.Enums;

namespace Banking.Core.Models
{
    public class BankTransaction
    {
        public Guid BankTransactionID { get; set; }

        public int AccountNumber { get; set; }

        public DateTime PostedDate { get; set; }

        public decimal PostedAmount { get; set; }

        public string Description { get; set; }

        public BankingTransactionType TransactionType {get; set;}

    }
}
