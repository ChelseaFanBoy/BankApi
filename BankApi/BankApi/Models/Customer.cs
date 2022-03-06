using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankApi.Models
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string CustomerName { get; set; } = null!;

        public int CustomerID { get; set; }

        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}
