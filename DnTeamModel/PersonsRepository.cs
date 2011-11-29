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
    /// Manages Persons entity manipulation
    /// </summary>
    public static class PersonsRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Person> Coll = Db.GetCollection<Person>("Persons");
        private static readonly MongoCollection<Department> Dpt = Db.GetCollection<Department>("Departments");

        /// <summary>
        /// Returns the list of persons
        /// </summary>
        /// <param name="isActive">true - returns active persons, false - inActive persons</param>
        /// <returns>The list of Persons</returns>
        public static List<Person> GetAllPersons(bool isActive)
        {
            var query = Query.EQ("IsActive", isActive);
            return Coll.Find(query).ToList();
        }

        /// <summary>
        /// Returns Dictioonary of persons with the first "wanted" person
        /// </summary>
        /// <returns>The dictionary of persons with the first "wanted" person</returns>
        public static Dictionary<string, string> GetActivePersonsList()
        {
            return new Dictionary<string, string> { {ObjectId.Empty.ToString(), "wanted"}}.Concat(
                GetAllPersons(true).ToDictionary(o => o.Id.ToString(), o => o.Name)).ToDictionary(x => x.Key, x => x.Value);
            
        }

        /// <summary>
        /// Create a new person
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="locatedIn">Location id</param>
        /// <param name="primaryManager">Primary manager id</param>
        /// <returns>Creation status of type PersonCreateStatus</returns>
        public static PersonCreateStatus CreatePerson(string userName, string locatedIn, string primaryManager)
        {
            ObjectId locationId;
            ObjectId managerId;
            var status = VerifyPerson(userName, locatedIn, primaryManager, out locationId, out managerId);
            if (status != PersonCreateStatus.Success) return status;

            var p = new Person
                        {
                            Name = userName,
                            LocatedIn = locationId,
                            PrimaryManager = managerId
                        };

            var res = Coll.Insert(p, SafeMode.True);

            return res.Ok ? PersonCreateStatus.Success : PersonCreateStatus.ProviderError;
        }

        private static PersonCreateStatus VerifyPerson(string userName, string locatedIn, string primaryManager, out ObjectId locationId, out ObjectId managerId)
        {
            locationId = ObjectId.Empty;
            managerId = ObjectId.Empty;

            var query = Query.EQ("Name", userName);
            if (Coll.Find(query).Any())
                return PersonCreateStatus.DuplicateUserName;

            managerId = string.IsNullOrEmpty(primaryManager) ? ObjectId.Empty : ObjectId.Parse(primaryManager);
            if (managerId != ObjectId.Empty)
            {
                query = Query.EQ("_id", managerId);
                if (!Coll.Find(query).Any())
                    return PersonCreateStatus.InvalidPrimaryManager;
            }

            locationId = string.IsNullOrEmpty(locatedIn) ? ObjectId.Empty : ObjectId.Parse(locatedIn);
            if (locationId != ObjectId.Empty)
            {
                query = Query.EQ("Subsidiaries._id", locationId);
                if (!Dpt.Find(query).Any())
                    return PersonCreateStatus.InvalidLocation;
            }

            return PersonCreateStatus.Success;
        }

        /// <summary>
        /// Returns the name of the specified person
        /// </summary>
        /// <param name="id">Person id</param>
        /// <returns>Person name</returns>
        public static string GetPersonName(ObjectId id)
        {
            if (id == ObjectId.Empty) return "wanted";

            var query = Query.EQ("_id", id);

            return Coll.FindOne(query).Name;
        }

        public static Person GetPerson(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));

            return Coll.FindOne(query);
        }

        public static string UpdateProperty(string id, string name, string value)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));

            dynamic val;
            switch (name)
            {
                case "DoB":
                    val = DateTime.Parse(value);
                    break;

                case "LocatedIn":
                    val = ObjectId.Parse(value);
                    break;

                case "PrimaryManager": goto case "LocatedIn";
                
                case "PrimaryPeer": goto case "LocatedIn";

                default:
                    val = value;
                    break;
            }

            var update = Update.Set(name, val);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? string.Empty : "error description"; //TODO: return user-friendly error
        }

        public static string AddValueToPropertySet(string id, string name, string value)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));

            dynamic val;
            switch (name)
            {
                case "Links":
                    val = value;
                    break;

                default:
                    val = ObjectId.Parse(value);
                    break;
            }

            var update = Update.AddToSet(name, val);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? string.Empty : "error"; //TODO: return user-friendly error
        }

        public static string DeleteValueFromPropertySet(string id, string name, string value)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));

            dynamic val;
            switch (name)
            {
                case "Links":
                    val = value;
                    break;

                default:
                    val = ObjectId.Parse(value);
                    break;
            }


            var update = Update.Pull(name, val);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? string.Empty : "error description"; //TODO: return user-friendly error
        }

        public static string AddTechnologySpecialty(string id, string name, int value, string lastUsed, string expSince, string note)
        {
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(id)), Query.NE("TechnologySpecialties.Name", name) });

            var update = Update.AddToSet("TechnologySpecialties", new Specialty
            {
                Name = name,
                Level = value, 
                LastUsed = DateTime.Parse(lastUsed), 
                ExperienceSince = DateTime.Parse(expSince), 
                LastProjectNote = note
            }.ToBsonDocument());

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? string.Empty : "error description"; //TODO: return user-friendly error
        }

        public static string DeleteTechnologySpecialty(string id, string name)
        {
            var update = Update.Pull("TechnologySpecialties", Query.EQ("Name", name));
            var res = Coll.Update(Query.EQ("_id", ObjectId.Parse(id)), update, SafeMode.True);

            return res.Ok ? string.Empty : "error description"; //TODO: return user-friendly error
        }

        public static object UpdateTechnologySpecialty(string id, string name, int value, string lastUsed, string expSince, string note)
        {
            
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(id)), Query.EQ("TechnologySpecialties.Name", name) });
            var update = Update.Set("TechnologySpecialties.$.Level", value).Set("TechnologySpecialties.$.LastUsed", DateTime.Parse(lastUsed))
                .Set("TechnologySpecialties.$.ExperienceSince", DateTime.Parse(expSince)).Set("TechnologySpecialties.$.LastProjectNote", note);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? string.Empty : "error description"; //TODO: return user-friendly error
        }

        public static void DeletePerson(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("IsActive", false);

            Coll.Update(query, update, SafeMode.True);
        }

        public static PersonCreateStatus UpdatePerson(string id, string userName, string locatedIn, string primaryManager)
        {
            ObjectId locationId;
            ObjectId managerId;
            var status = VerifyPerson(userName, locatedIn, primaryManager, out locationId, out managerId);
            if (status != PersonCreateStatus.Success) return status;

            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", userName).Set("PrimaryManager", managerId).Set("LocatedIn", locationId);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? PersonCreateStatus.Success : PersonCreateStatus.ProviderError;
        }

        /// <summary>
        /// Validates if the OpenId is assigned to a user
        /// </summary>
        /// <param name="id">OpenId (is "http" or "https" URI, or an XRI)</param>
        /// <returns>User name</returns>
        public static string ValidateIdentifier(string id)
        {
            var query = Query.And(new [] {Query.EQ("OpenId", id), Query.EQ("IsActive", true)});
            var res = Coll.FindOne(query);

            return (res == null) ? string.Empty : res.Name;
        }

        internal static IEnumerable<ObjectId> GetUsedDepartments(IEnumerable<ObjectId> departments)
        {
            var query = Query.In("LocatedIn", new BsonArray(departments));
            var result = Coll.Find(query);
            result.Fields = Fields.Include(new[] { "LocatedIn" });

            return result.Select(o => o.LocatedIn);
        }
    }
}
