using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly BankService _bankService;

        public BankController(BankService bankService) =>
        _bankService = bankService;

        /// <summary>
        /// Gets information of all customers
        /// </summary>
        /// <returns>List of customer details</returns>
        [HttpGet("GetAllCustomerDetails")]
        public async Task<List<Customer>> GetAllCustomerDetails() =>
        await _bankService.GetAsync();

        /// <summary>
        /// Gets customer information based on customerID
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>Details of a single customer</returns>
        [HttpGet("GetCustomerDetails")]
        public async Task<ActionResult<Customer>> GetCustomerDetails(int customerId)
        {
            var customer = await _bankService.GetAsync(customerId);

            if (customer is null)
            {
                return NotFound();
            }

            return customer;
        }

        /// <summary>
        /// Adds a new customer detail
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost("AddNewCustomer")]
        public async Task<IActionResult> AddNewCustomer(Customer customer)
        {
            await _bankService.CreateAsync(customer);

            return CreatedAtAction(nameof(GetAllCustomerDetails), new { id = customer.CustomerID }, customer);
        }

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
            //Add the new account
            customer.Accounts.Add(accountDetails);

            await _bankService.UpdateAsync(customerId, customer);

            return NoContent();
        }

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
            var account = customer.Accounts.Single(x=>x.AccountNumber == accountNumber);
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
            foreach(var account in customer.Accounts)
            {
                List<Transaction> trx = account.Transactions.
                    Where(x => x.TransactionTime >= startTime && x.TransactionTime <= endTime).ToList();
                transaction.AddRange(trx);
            }

            return transaction;
        }
    }
}
