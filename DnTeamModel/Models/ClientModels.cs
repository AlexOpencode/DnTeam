using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    class Client
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ClientId
        {
            get { return Id.ToString(); }
        }
        public string Name { get; set; }
        public List<string> Links { get; set; }
        public List<Product> Products { get; set; }
        
        Client()
        {
            Id = ObjectId.GenerateNewId();
            Products = new List<Product>();
        }
    }

    class Product
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ProductId
        {
            get { return Id.ToString(); }
        }
        public string Name { get; set; }
        public List<ObjectId> Projects { get; set; }

        Product()
        {
            Id = ObjectId.GenerateNewId();
            Projects = new List<ObjectId>();
        }
    }
}
