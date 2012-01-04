using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    /// <summary>
    /// A static class that manages Products
    /// </summary>
    public static class ProductRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Product> _coll = Db.GetCollection<Product>("Products");

        #if DEBUG //Test area
        /// <summary>
        /// Set the name of the collection
        /// </summary>
        /// <param name="collectionName">Collection name</param>
        public static void SetTestCollection(string collectionName)
        {
            _coll = Db.GetCollection<Product>(collectionName);
        }
        
        /// <summary>
        /// Method is used to test internal GetName method
        /// </summary>
        public static string GetNameTest(ObjectId id)
        {
            return GetName(id);
        }

        /// <summary>
        /// Method is used to test internal GetName method
        /// </summary>
        public static IEnumerable<ObjectId> GetUsedClientsTest()
        {
            return GetUsedClients();
        }
        #endif


        /// <summary>
        /// Returns the list of all products
        /// </summary>
        /// <returns>The list of products</returns>
        public static List<Product> GetAllProducts()
        {
            return _coll.FindAll().ToList();
        }

        /// <summary>
        /// Inserts a new product
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="client">Id of existing Client or Name of a new one</param>
        /// <param name="isClientNew">True - if client is a new one</param>
        /// <returns>Transaction Status</returns>
        public static ProductEditStatus InsertProduct(string name, string client, bool isClientNew)
        {
            if (string.IsNullOrEmpty(name))
                return ProductEditStatus.NameIsEmpty;

            ObjectId clientId;
            if (isClientNew) 
            {
                //Add a new client
                clientId = ClientRepository.InsertClient(client);
            }
            else
            {
                if(!ObjectId.TryParse(client, out clientId))
                    return ProductEditStatus.ClientIsInvalid;
            }

            try
            {
                _coll.Insert(new Product { Name = name, ClientId = clientId }, SafeMode.True);
            }
            catch(MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return ProductEditStatus.DuplicateItem;

                throw;
            }

            return ProductEditStatus.Ok;
        }

        /// <summary>
        /// Updates the product's name and client values
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="name">Product Name</param>
        /// <param name="client">Id of existing Client or Name of a new one</param>
        /// <param name="isClientNew">True - if client is a new one</param>
        /// <returns>Transaction Status</returns>
        public static ProductEditStatus UpdateProduct(string id, string name, string client, bool isClientNew)
        {
            if (string.IsNullOrEmpty(name))
                return ProductEditStatus.NameIsEmpty;

            ObjectId clientId;
            if (isClientNew)
            {
                //Add a new client
                clientId = ClientRepository.InsertClient(client);
            }
            else
            {
                if (!ObjectId.TryParse(client, out clientId))
                    return ProductEditStatus.ClientIsInvalid;
            }

            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", name).Set("ClientId", clientId);
            
            try
            {
            _coll.Update(query, update, SafeMode.True);
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return ProductEditStatus.DuplicateItem;

                throw;
            }

            return ProductEditStatus.Ok;
        }

        /// <summary>
        /// Deletes defined products, except the ones, used by projects
        /// </summary>
        /// <param name="values">The list of products</param>
        public static void DeleteProducts(IEnumerable<string> values)
        {
            //Get used product ids
            var used = ProjectRepository.GetUsedProducts();

            var query = Query.In("_id", new BsonArray(values.Select(ObjectId.Parse).Except(used)));

            _coll.Remove(query);
        }

        /// <summary>
        /// Returns the name of the defined product
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns>Product name</returns>
        internal static string GetName(ObjectId id)
        {
            return _coll.FindOneById(id).Name;
        }

        /// <summary>
        /// Returns the dictionary of all products
        /// </summary>
        /// <returns>Dictionary(Product id, Product name)</returns>
        public static Dictionary<string, string> GetAllProductsDictionary()
        {
            return GetAllProducts().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        /// <summary>
        /// Returns the list of Client Ids used linked to Products
        /// </summary>
        /// <returns>The list of Client Ids</returns>
        internal static IEnumerable<ObjectId> GetUsedClients()
        {
            var cursor = _coll.FindAll();
            cursor.Fields = Fields.Include("ClientId");
            
            return cursor.Select(o=>o.ClientId);
        }
    }
}
