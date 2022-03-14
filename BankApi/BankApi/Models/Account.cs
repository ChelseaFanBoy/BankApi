using BankApi.Models.Enums;

namespace BankApi.Models
{
    public class Account
    {
        public AccountType AccountType { get; set; }

        public int AccountNumber { get; set; }

        public int CurrentBalance { get; set; }

        public TransactionType LastTransaction { get; set; }

        public DateTime LastTransactionTime { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
