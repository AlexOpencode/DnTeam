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
            return Coll.FindAll().ToList();
        }

        public static Dictionary<string, string> GetPersonsList()
        {
            return GetAllPersons().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }


        public static UserCreateStatus CreatePerson(string userName, string locatedIn, string primaryManager, string email)
        {
            var query = Query.EQ("Name", userName);
            if (Coll.Find(query).Any())
                return UserCreateStatus.DuplicateUserName;

            query = Query.EQ("Email", email);
            if (Coll.Find(query).Any())
                return UserCreateStatus.DuplicateEmail;

            ObjectId managerId = string.IsNullOrEmpty(primaryManager) ? ObjectId.Empty : ObjectId.Parse(primaryManager);
            if (managerId != ObjectId.Empty)
            {
                query = Query.EQ("_id", managerId);
                if (!Coll.Find(query).Any())
                    return UserCreateStatus.InvalidPrimaryManager;
            }

            ObjectId locationId = string.IsNullOrEmpty(locatedIn) ? ObjectId.Empty : ObjectId.Parse(locatedIn);
            if (locationId != ObjectId.Empty)
            {
                query = Query.EQ("Subsidiaries._id", locationId);
                if (!Dpt.Find(query).Any())
                    return UserCreateStatus.InvalidLocation;
            }

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

        public static string GetUserName(ObjectId id)
        {
            var query = Query.EQ("_id", id);

            return Coll.FindOne(query).Name;
        }

        //public static void CreateUser(string userName, string email, string location, string password, 
        //    string primaryManager, string comments,  List<Specialty> technologySklills, DateTime DoB, 
        //    bool isLocked, out UserCreateStatus createStatus)
        //{
        //   

        //    var query = Query.EQ("Name", userName);
        //    if(coll.Find(query).Count() > 0)
        //    {
        //        createStatus = UserCreateStatus.DuplicateUserName;
        //        return;
        //    }

        //    query = Query.EQ("Email", email);
        //    if (coll.Find(query).Count() > 0)
        //    {
        //        createStatus = UserCreateStatus.DuplicateEmail;
        //        return;
        //    }

        //    ObjectId managerId;
        //    if (ObjectId.TryParse(primaryManager, out managerId))
        //    {
        //        query = Query.EQ("_id", managerId);
        //        if (coll.Find(query).Count() != 1)
        //        {
        //            createStatus = UserCreateStatus.InvalidPrimaryManager;
        //            return;
        //        }
        //    }

        //    var res = coll.Insert(new Person ()
        //                   {
        //                       Comments = comments, 
        //                       DoB = DoB, 
        //                       Email = email, 
        //                       IsLocked = isLocked, 
        //                       Name = userName,
        //                       Password = password,
        //                       PrimaryManager = managerId,
        //                       TechnologySpecialties = technologySklills,
        //                       Location = location
        //                   }, SafeMode.True);
        //    if (res.Ok)
        //    {
        //        createStatus = UserCreateStatus.Success;
        //        return;
        //    }

        //    createStatus = UserCreateStatus.UserRejected;
        //}



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
    }
}
