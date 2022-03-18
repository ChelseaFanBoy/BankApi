using BankApi.Models;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IBankService _bankService;

        public CustomerController(IBankService bankService) =>
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
            //Create new Guid for inserting into mongoDB
            customer.Id = Guid.NewGuid().ToString();
            //CustomerID is next number of latest customer's ID
            var lastCustomer = _bankService.GetAsync().Result.OrderByDescending(x=>x.CustomerID).FirstOrDefault();
            customer.CustomerID = lastCustomer is null ? 1 : ++lastCustomer.CustomerID;

            SetAccountDetails(ref customer);

            await _bankService.CreateAsync(customer);

            return CreatedAtAction(nameof(GetAllCustomerDetails), new { id = customer.CustomerID }, customer);
        }

        private static void SetAccountDetails(ref Customer customer)
        {
            foreach(var account in customer.Accounts)
            {
                foreach (var transaction in account.Transactions)
                {
                    if (transaction.TransactionType == Models.Enums.TransactionType.Credit)
                        account.CurrentBalance += transaction.Amount;
                    else if(transaction.TransactionType == Models.Enums.TransactionType.Debit)
                        account.CurrentBalance -= transaction.Amount;
                }
                var lastTransaction = account.Transactions.OrderByDescending(x => x.TransactionTime).First();
                account.LastTransaction = lastTransaction.TransactionType;
            }
        }
    }
}
