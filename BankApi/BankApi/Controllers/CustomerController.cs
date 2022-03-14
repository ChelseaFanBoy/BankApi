using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BankService _bankService;

        public CustomerController(BankService bankService) =>
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
    }
}
