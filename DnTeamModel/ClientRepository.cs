using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace DnTeamData
{
    public static class ClientRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Client> Coll = Db.GetCollection<Client>("Clients");

        public static List<Client> GetAllClients()
        {
            return Coll.FindAll().ToList();
        }

        public static Dictionary<string, string> GetClientsList()
        {
            return GetAllClients().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        //public static IEnumerable<string> GetClientsList()
        //{
        //    return GetAllClients().Select(x => x.Name);
        //}

        public static Client GetClient(ObjectId id)
        {
            var query = Query.EQ("_id", id);
            return Coll.FindOne(query);
        }

        public static void InsertClient(string name)
        {
            Coll.Insert(new Client { Name = name }, SafeMode.True);
        }

        public static void UpdateClient(string id, string name)
        {

            var query = Query.EQ("_id", id);
            var update = Update.Set("Name", name);
            Coll.Update(query, update);
        }

        public static void DeleteClient(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            Coll.Remove(query);
        }

        public static void AddLink(string id, string link)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.AddToSet("Links", link);
            Coll.Update(query, update);
        }

        public static void RemoveLink(string id, string link)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Pull("Links", link);
            Coll.Update(query, update);
        }

        public static void InsertBatchClient(string value)
        {
            try
            {
                var options = new MongoInsertOptions(Coll)
                                  {
                                      CheckElementNames = true,
                                      Flags = InsertFlags.ContinueOnError,
                                      SafeMode = SafeMode.True
                                  };
                Coll.InsertBatch(value.Split('~').Where(o => !string.IsNullOrEmpty(o)).Select(o => new Client {Name = o.Trim()}), options);
            }
            catch(MongoSafeModeException)
            {
               //TODO: get appropriate error code
            }
        }
    }
}
