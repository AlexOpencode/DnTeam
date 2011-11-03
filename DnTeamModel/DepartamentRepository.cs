using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    public static class DepartamentRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Departament> Coll = Db.GetCollection<Departament>("Departaments");

        #if DEBUG //Sets collection name for tests
        public static void SetTestCollection(string collectionName)
        {
            Coll = Db.GetCollection<Departament>(collectionName);
        }
        #endif


        public static List<Departament> GetAllDepartaments()
        {
            var query = Query.EQ("IsDeleted", false);
            return Coll.Find(query).ToList();
        }

        public static Dictionary<string, string> GetDepartamentsList()
        {
            return GetAllDepartaments().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        public static IEnumerable<TDepartament> GetDepartamentsTree()
        {
            
            var departaments = GetAllDepartaments();
            //Get all top departaments
            var topDepartaments = departaments.Where(o => o.DepartamentOf == ObjectId.Empty).ToList();
            //Reduse total departaments
            departaments = departaments.Except(topDepartaments).ToList();
            //Convert Departaments to TDepartament
            var resDepartaments = topDepartaments.Select(o => new TDepartament
            {
                Id = o.Id.ToString(),
                Name = o.Name,
                Subsidaries = o.Subsidaries.Where(x=>x.IsDeleted == false).ToList()
            }).ToList();

            //Retrive sub-departaments
            GetSubDepartaments(departaments, resDepartaments);

            return resDepartaments;
        }

        private static void GetSubDepartaments(List<Departament> departaments, List<TDepartament> resDepartaments)
        {
            foreach (TDepartament departament in resDepartaments)
            {
                var subDepartaments = departaments.Where(o => o.DepartamentOf == ObjectId.Parse(departament.Id)).ToList();
                if (subDepartaments.Count() <= 0) continue;

                departament.SubDepartaments = subDepartaments.Select(o => new TDepartament
                                                                              {
                                                                                  Id = o.Id.ToString(),
                                                                                  Name = o.Name,
                                                                                  Subsidaries = o.Subsidaries
                                                                              }).ToList();

                departaments = departaments.Except(subDepartaments).ToList();

                GetSubDepartaments(departaments, departament.SubDepartaments);
            }

            
        }
        
        public static bool CreateDepartament(string name, string departamentOf)
        {
           var res = Coll.Insert(new Departament
                                     {
                                          Name = name,
                                          DepartamentOf = string.IsNullOrEmpty(departamentOf) ? ObjectId.Empty : ObjectId.Parse(departamentOf)
                                      }, SafeMode.True);
            return res.Ok;
        }


        public static bool CreateSubsidary(string location, float baseCost, float baseRate, string departamentId)
        {
            var query = Query.EQ("_id", ObjectId.Parse(departamentId));
            var dep = Coll.FindOne(query);

            //Verify location is unique
            if (dep.Subsidaries.Any(o => o.Location == location && o.IsDeleted == false))
                return false;

            dep.Subsidaries.Add(new Subsidary
                                    {
                                        Location = location,
                                        BaseCost = baseCost,
                                        BaseRate = baseRate
                                    });

            var update = Update.Replace(dep);
            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }

        public static bool DeleteDepartament(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("IsDeleted", true);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }

        public static bool DeleteSubsidary(string id, string departamentId)
        {
            var query = Query.EQ("_id", ObjectId.Parse(departamentId));
            var dep = Coll.FindOne(query);

            dep.Subsidaries.Single(o => o.Id == ObjectId.Parse(id)).IsDeleted = true;

            var update = Update.Replace(dep);
            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }


        public static bool UpdateDepartament(string id, string name)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", name);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }

        public static bool UpdateSubsidary(string id, string location, float baseCost, float baseRate, string parentId)
        {
            var query = Query.EQ("_id", ObjectId.Parse(parentId));
            var dep = Coll.FindOne(query);

            dep.Subsidaries.Single(o => o.Id == ObjectId.Parse(id)).Location = location;
            dep.Subsidaries.Single(o => o.Id == ObjectId.Parse(id)).BaseCost = baseCost;
            dep.Subsidaries.Single(o => o.Id == ObjectId.Parse(id)).BaseRate = baseRate;

            var update = Update.Replace(dep);
            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }
    }
}
