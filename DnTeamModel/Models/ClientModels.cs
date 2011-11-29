using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    public class Client
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ClientId { get { return Id.ToString(); } }
        public string Name { get; set; }
        public List<string> Links { get; set; }
        
        public Client()
        {
            //Id = ObjectId.GenerateNewId();
        }
    }

    public class Product
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ProductId { get { return Id.ToString(); } }
        public string Name { get; set; }
        public ObjectId ClientId { get; set; }
        public string ClientName ()
        {
            return ClientRepository.GetClient(ClientId).Name;
        }

        public Product()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
