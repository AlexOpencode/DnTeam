using System;
using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    public static class PersonsRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Person> Coll = Db.GetCollection<Person>("Persons");
        private static readonly MongoCollection<Department> Dpt = Db.GetCollection<Department>("Departments");

        public static List<Person> GetAllPersons()
        {
            var query = Query.EQ("IsDeleted", false);
            return Coll.Find(query).ToList();
        }

        public static Dictionary<string, string> GetPersonsList()
        {
            return GetAllPersons().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        public static UserCreateStatus CreatePerson(string userName, string locatedIn, string primaryManager, string email)
        {
            ObjectId locationId;
            ObjectId managerId;
            var status = VerifyPerson(userName, locatedIn, primaryManager, email, out locationId, out managerId);
            if (status != UserCreateStatus.Success) return status;

            var p = new Person
                        {
                            Name = userName,
                            LocatedIn = locationId,
                            PrimaryManager = managerId,
                            Email = email
                        };

            var res = Coll.Insert(p, SafeMode.True);

            return res.Ok ? UserCreateStatus.Success : UserCreateStatus.ProviderError;
        }

        private static UserCreateStatus VerifyPerson(string userName, string locatedIn, string primaryManager, string email, out ObjectId locationId, out ObjectId managerId)
        {
            locationId = ObjectId.Empty;
            managerId = ObjectId.Empty;

            var query = Query.EQ("Name", userName);
            if (Coll.Find(query).Any())
                return UserCreateStatus.DuplicateUserName;

            query = Query.EQ("Email", email);
            if (Coll.Find(query).Any())
                return UserCreateStatus.DuplicateEmail;

            managerId = string.IsNullOrEmpty(primaryManager) ? ObjectId.Empty : ObjectId.Parse(primaryManager);
            if (managerId != ObjectId.Empty)
            {
                query = Query.EQ("_id", managerId);
                if (!Coll.Find(query).Any())
                    return UserCreateStatus.InvalidPrimaryManager;
            }

            locationId = string.IsNullOrEmpty(locatedIn) ? ObjectId.Empty : ObjectId.Parse(locatedIn);
            if (locationId != ObjectId.Empty)
            {
                query = Query.EQ("Subsidiaries._id", locationId);
                if (!Dpt.Find(query).Any())
                    return UserCreateStatus.InvalidLocation;
            }

            return UserCreateStatus.Success;
        }

        public static string GetUserName(ObjectId id)
        {
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

            return res.Ok ? string.Empty : "error description"; //TODO: return user-friendly error
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
            var update = Update.Set("IsDeleted", true);

            Coll.Update(query, update, SafeMode.True);
        }

        public static UserCreateStatus UpdatePerson(string id, string userName, string locatedIn, string primaryManager, string email)
        {
            ObjectId locationId;
            ObjectId managerId;
            var status = VerifyPerson(userName, locatedIn, primaryManager, email, out locationId, out managerId);
            if (status != UserCreateStatus.Success) return status;

            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", userName).Set("PrimaryManager", managerId).Set("Email", email).Set("LocatedIn", locationId);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? UserCreateStatus.Success : UserCreateStatus.ProviderError;
        }
    }
}
