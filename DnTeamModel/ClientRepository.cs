﻿using System;
using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace DnTeamData
{
    /// <summary>
    /// A static class that manages Clients
    /// </summary>
    public static class ClientRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Client> _coll = Db.GetCollection<Client>("Clients");

        #if DEBUG //Test variables
        /// <summary>
        /// Set the name of the collection
        /// </summary>
        /// <param name="collectionName">Collection name</param>
        public static void SetTestCollection(string collectionName)
        {
            _coll = Db.GetCollection<Client>(collectionName);
        }
        
        #endif

        /// <summary>
        /// Inserts a new client to the database
        /// </summary>
        /// <param name="client">Client object</param>
        public static TransactionStatus InsertClient(Client client)
        {
            try
            {
                _coll.Insert(client, SafeMode.True);
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return TransactionStatus.DuplicateItem;

                throw;
            }

            return TransactionStatus.Ok;
        }

        /// <summary>
        /// Returns the list of all clients
        /// </summary>
        /// <returns>The list of all clients</returns>
        public static IEnumerable<Client> GetAllClients()
        {
            return _coll.FindAll().ToList();
        }

        /// <summary>
        /// Returns a dictionary of clients
        /// </summary>
        /// <returns>Dictionary(id,name)</returns>
        public static Dictionary<string, string> GetClientsDictionary()
        {
            return GetAllClients().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        /// <summary>
        /// Returns the name of the defined client
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>Name</returns>
        public static string GetName(ObjectId id)
        {
            return _coll.FindOneById(id).Name;
        }

        /// <summary>
        /// Updates client name
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="name">Client Name</param>
        /// <returns>Update status</returns>
        public static TransactionStatus UpdateClient(string id, string name)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", name);
            try
            {
                _coll.Update(query, update, SafeMode.True);
            }
            catch(MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return TransactionStatus.DuplicateItem;

                throw;
            }

            return TransactionStatus.Ok;
        }

        /// <summary>
        /// Deletes defined clients except ones, used on Products
        /// </summary>
        /// <param name="values">Selected clients</param>
        public static void DeleteClients(IEnumerable<string> values)
        {
            //Get Clients used on Products
            var productClients = ProductRepository.GetUsedClients();

            var query = Query.In("_id", new BsonArray(values.Select(ObjectId.Parse).Except(productClients)));
            _coll.Remove(query);
        }
       
        /// <summary>
        /// Inserts the list of clients, except ones whith the duplicate name
        /// </summary>
        /// <param name="values">Client names</param>
        public static void InsertClients(IEnumerable<string> values)
        {
            var options = new MongoInsertOptions(_coll)
                              {
                                  CheckElementNames = true,
                                  Flags = InsertFlags.ContinueOnError,
                                  SafeMode = SafeMode.False
                              }; //if name is dublicate skips it, and continue to insert others

            _coll.InsertBatch(values.Select(o => new Client{ Name = o }), options);
        }
    }
}
