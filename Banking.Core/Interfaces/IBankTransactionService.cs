using System;
using System.Collections.Generic;
using System.Text;

using Banking.Core.Models;

namespace Banking.Core.Interfaces
{
    public interface IBankTransactionService
    {
        BankTransaction AddTransaction(BankTransaction bankTransaction);
        public List<BankTransaction> GetAllBankingTransactions(int accountNumber);
        public decimal GetBalance(int accountNumber);

    }
}
