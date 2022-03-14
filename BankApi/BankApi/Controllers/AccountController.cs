using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BankService _bankService;

        public AccountController(BankService bankService) =>
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
            //Add the new account
            customer.Accounts.Add(accountDetails);

            await _bankService.UpdateAsync(customerId, customer);

            return NoContent();
        }
    }
}
