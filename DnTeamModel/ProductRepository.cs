using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    public static class ProductRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Product> Coll = Db.GetCollection<Product>("Products");

        public static List<Product> GetAllProducts()
        {
            return Coll.FindAll().ToList();
        }
        public static void InsertProduct(string name, string clientId)
        {
            Coll.Insert(new Product()
            {
                Name = name,
                ClientId = ObjectId.Parse(clientId)
            });
        }

        public static void UpdateProduct(string id, string name, string clientId)
        {

            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", name).Set("ClientId", ObjectId.Parse(clientId));
            Coll.Update(query, update);
        }

        public static void DeleteProduct(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            Coll.Remove(query);
        }
    }
}
