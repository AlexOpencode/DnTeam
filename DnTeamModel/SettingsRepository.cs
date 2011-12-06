using System;
using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    /// <summary>
    /// A static class that manages settings in the Enums collection
    /// </summary>
    public static class SettingsRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Enums> _coll = Db.GetCollection<Enums>("Enums");

        #if DEBUG //Test variables
        /// <summary>
        /// Set the name of the collection
        /// </summary>
        /// <param name="collectionName">Departments collection name</param>
        public static void SetTestCollection(string collectionName)
        {
            _coll = Db.GetCollection<Enums>(collectionName);
        }
        #endif

        /// <summary>
        /// Returns parsed string as Enum
        /// </summary>
        /// <param name="value">Settings name</param>
        /// <returns>Enum value</returns>
        public static EnumName GetEnumName(string value)
        {
            EnumName res;
            if (Enum.TryParse(value.Replace(" ", string.Empty), true, out res))
                return res;

            return EnumName.Undefined;
        }

        /// <summary>
        /// Returns the list of values of the defined setting
        /// </summary>
        /// <param name="name">Setting's Enum value</param>
        /// <returns>The list of settings</returns>
        public static List<string> GetSettingValues(EnumName name)
        {
            var query = Query.EQ("_id", name.ToString());
            var collection = _coll.FindOne(query);
            
            return collection.Values ?? new List<string>();
        }

        /// <summary>
        /// Adds value (skip dublicate values) to the defined setting. 
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <param name="value">Value to add</param>
        public static void AddSettingValue(EnumName name, string value)
        {
            var query = Query.EQ("_id", name.ToString());
            var update = Update.AddToSet("Values", value);

            _coll.Update(query, update);
        }

        /// <summary>
        /// Adds the list of values to the defined Setting
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <param name="values">Values string</param>
        public static void BatchAddSettingValues(EnumName name, IEnumerable<string> values)
        {
            var query = Query.EQ("_id", name.ToString());
            var update = Update.AddToSetEach("Values", BsonArray.Create(values));

            _coll.Update(query, update);
        }

        /// <summary>
        /// Removes all values from the defined Setting
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <param name="values">Values string</param>
        public static void BatchDeleteSettingValues(EnumName name, IEnumerable<string> values)
        {
            var query = Query.EQ("_id", name.ToString());
            var update = Update.PullAll("Values", BsonArray.Create(values));

            _coll.Update(query, update);
        }
    }
}
