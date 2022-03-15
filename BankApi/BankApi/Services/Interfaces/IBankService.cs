using BankApi.Models;

namespace BankApi.Services.Interfaces
{
    public interface IBankService
    {
        Task<List<Customer>> GetAsync();

        Task<Customer?> GetAsync(int customerID);

        Task CreateAsync(Customer newCustomer);

        Task UpdateAsync(int customerID, Customer updatedCustomer);
    }
}
