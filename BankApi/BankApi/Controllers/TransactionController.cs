using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly BankService _bankService;

        public TransactionController(BankService bankService) =>
        _bankService = bankService;

        /// <summary>
        /// Performs a transaction against a particular customerId and accountNumber
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="accountNumber"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPut("PerformTransaction")]
        public async Task<IActionResult> PerformTransaction(int customerId, int accountNumber, Transaction transaction)
        {
            var customer = await _bankService.GetAsync(customerId);

            if (customer is null)
            {
                return NotFound();
            }

            //Cannot perform trx if account does not exist
            if (!customer.Accounts.Any(x => x.AccountNumber == accountNumber))
                return BadRequest();

            //Update the transaction
            var account = customer.Accounts.Single(x => x.AccountNumber == accountNumber);
            int index = customer.Accounts.IndexOf(account);

            account.Transactions.Add(transaction);
            account.LastTransaction = transaction.TransactionType;
            account.LastTransactionTime = transaction.TransactionTime;
            account.CurrentBalance += transaction.Amount;

            customer.Accounts[index] = account;

            await _bankService.UpdateAsync(customerId, customer);

            return NoContent();
        }

        /// <summary>
        /// Endpoint to find the transactions in a particular time period
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("GetTransactionDetails")]
        public async Task<List<Transaction>> GetTransactionDetails(int customerId,
                                                                            DateTime startTime, DateTime endTime)
        {
            var customer = await _bankService.GetAsync(customerId);

            if (customer is null)
            {
                return new List<Transaction>();
            }

            //LINQ to find the transactions in a particular time period
            var transaction = new List<Transaction>();
            foreach (var account in customer.Accounts)
            {
                List<Transaction> trx = account.Transactions.
                    Where(x => x.TransactionTime >= startTime && x.TransactionTime <= endTime).ToList();
                transaction.AddRange(trx);
            }

            return transaction;
        }
    }
}
