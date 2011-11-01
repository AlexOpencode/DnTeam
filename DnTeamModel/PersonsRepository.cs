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
        
        public static List<Person> GetAllUsers()
        {
            MongoCollection<Person> coll = Db.GetCollection<Person>("Users");
           
            return coll.FindAll().ToList();
        }

        public static Dictionary<string, string> GetUsersList()
        {
            return GetAllUsers().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        public static void CreateUser(string userName, string email, string location, string password, 
            string primaryManager, string comments,  List<Specialty> technologySklills, DateTime DoB, 
            bool isLocked, out UserCreateStatus createStatus)
        {
            MongoCollection<Enums> coll = Db.GetCollection<Enums>("Users");
            
            var query = Query.EQ("Name", userName);
            if(coll.Find(query).Count() > 0)
            {
                createStatus = UserCreateStatus.DuplicateUserName;
                return;
            }

            query = Query.EQ("Email", email);
            if (coll.Find(query).Count() > 0)
            {
                createStatus = UserCreateStatus.DuplicateEmail;
                return;
            }

            ObjectId managerId;
            if (ObjectId.TryParse(primaryManager, out managerId))
            {
                query = Query.EQ("_id", managerId);
                if (coll.Find(query).Count() != 1)
                {
                    createStatus = UserCreateStatus.InvalidPrimaryManager;
                    return;
                }
            }

            var res = coll.Insert(new Person ()
                           {
                               Comments = comments, 
                               DoB = DoB, 
                               Email = email, 
                               IsLocked = isLocked, 
                               Name = userName,
                               Password = password,
                               PrimaryManager = managerId,
                               TechnologySpecialties = technologySklills,
                               Location = location
                           }, SafeMode.True);
            if (res.Ok)
            {
                createStatus = UserCreateStatus.Success;
                return;
            }

            createStatus = UserCreateStatus.UserRejected;
        }

        public static string GetUserName(ObjectId id)
        {
            MongoCollection<Person> coll = Db.GetCollection<Person>("Users");
            var query = Query.EQ("_id", id);

            return coll.FindOne(query).Name;
        }
    }
}
