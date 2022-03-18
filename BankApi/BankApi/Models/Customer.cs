using MongoDB.Bson.Serialization.Attributes;

namespace BankApi.Models
{
    [BsonIgnoreExtraElements]
    public class Customer
    {
        [BsonId]
        public string? Id { get; set; }

        public string CustomerName { get; set; } = null!;

        public int CustomerID { get; set; }

        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}
