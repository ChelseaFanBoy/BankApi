using BankApi.Models;
using BankApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankApi.Services
{
    public class BankService : IBankService
    {
        private readonly IMongoCollection<Customer> _customerCollection;

        public BankService(
            IOptions<BankDatabaseSettings> bankDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bankDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bankDatabaseSettings.Value.DatabaseName);

            _customerCollection = mongoDatabase.GetCollection<Customer>(
                bankDatabaseSettings.Value.CustomerCollectionName);
        }

        public async Task<List<Customer>> GetAsync() =>
            await _customerCollection.Find(_ => true).ToListAsync();

        public async Task<Customer?> GetAsync(int customerID) =>
            await _customerCollection.Find(x => x.CustomerID == customerID)
            .FirstOrDefaultAsync();

        public async Task CreateAsync(Customer newCustomer) =>
            await _customerCollection.InsertOneAsync(newCustomer);

        public async Task UpdateAsync(int customerID, Customer updatedCustomer) =>
            await _customerCollection.ReplaceOneAsync(x => x.CustomerID == customerID, 
                updatedCustomer);
    }
}