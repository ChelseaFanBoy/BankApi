using BankApi.Models.Enums;

namespace BankApi.Models
{
    public class Transaction
    {
        public TransactionType TransactionType { get; set; }

        public int Amount { get; set; }

        public DateTime TransactionTime { get; set; }  = DateTime.UtcNow;
    }
}
