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

        //TODO: update references to the product
        public static void DeleteProduct(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            Coll.Remove(query);
        }

        internal static string GetName(ObjectId id)
        {
            return Coll.FindOneById(id).Name;
        }

        public static Dictionary<string,string> GetAllProductsList()
        {
            return GetAllProducts().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        /// <summary>
        /// Returns the list of Client Ids used linked to Products
        /// </summary>
        /// <returns>The list of Client Ids</returns>
        internal static IEnumerable<ObjectId> GetUsedClients()
        {
            var cursor = Coll.FindAll();
            cursor.Fields = Fields.Include("ClientId");
            
            return cursor.Select(o=>o.ClientId);
        }
    }
}
