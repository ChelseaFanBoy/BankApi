using BankApi.Models;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IBankService _bankService;

        public AccountController(IBankService bankService) =>
        _bankService = bankService;

        /// <summary>
        /// EndPoint to add a new account to existing customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="accountDetails"></param>
        /// <returns></returns>
        [HttpPut("AddAccount")]
        public async Task<IActionResult> AddAccount(int customerId, Account accountDetails)
        {
            var customer = await _bankService.GetAsync(customerId);

            if (customer is null)
            {
                return NotFound();
            }

            //Cannot add new account if account type already exists, or trying to add -ve balance
            if (customer.Accounts.Any(x => x.AccountType == accountDetails.AccountType) ||
                accountDetails.CurrentBalance < 0)
                return BadRequest();

            var account = customer.Accounts.OrderByDescending(x=>x.AccountNumber).FirstOrDefault();
            accountDetails.AccountNumber = account is null ? 1 : account.AccountNumber + 1;

            SetAccountDetails(ref accountDetails);

            //Add the new account
            customer.Accounts.Add(accountDetails);

            await _bankService.UpdateAsync(customerId, customer);

            return NoContent();
        }

        private static void SetAccountDetails(ref Account account)
        {
            foreach (var transaction in account.Transactions)
            {
                if (transaction.TransactionType == Models.Enums.TransactionType.Credit)
                    account.CurrentBalance += transaction.Amount;
                else if (transaction.TransactionType == Models.Enums.TransactionType.Debit)
                    account.CurrentBalance -= transaction.Amount;
            }
            var lastTransaction = account.Transactions.OrderByDescending(x => x.TransactionTime).First();
            account.LastTransaction = lastTransaction.TransactionType;
        }
    }
}
