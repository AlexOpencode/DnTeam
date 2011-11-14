using System;
using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    public static class ProjectRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Project> Coll = Db.GetCollection<Project>("Projects");

        #region Assignments

        public static List<Assignment> GetAssignments(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            
            return Coll.FindOne(query).Assignments.Where(o=>o.IsDeleted == false).ToList();
        }

        public static string CreateAssignment(string projectId, DateTime startDate, DateTime endDate, string person, string role, int commitment, string note)
        {
            var query = Query.EQ("_id", ObjectId.Parse(projectId));
            var update = Update.AddToSet("Assignments", new Assignment
            {
                StartDate = startDate,
                EndDate = endDate,
                Role = role,
                Person = ObjectId.Parse(person),
                Commitment = commitment,
                Note = note
            }.ToBsonDocument());
            
            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? "Ok" : "error";
        }

        public static string UpdateAssignment(string projectId, string id, DateTime startDate, DateTime endDate, string person, string role, int commitment, string note)
        {
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(projectId)), Query.EQ("Assignments._id", ObjectId.Parse(id)) });
            var update = Update.Set("Assignments.$.StartDate", startDate).Set("Assignments.$.EndDate", endDate).Set("Assignments.$.Person", ObjectId.Parse(person))
                .Set("Assignments.$.Role", role).Set("Assignments.$.Commitment", commitment).Set("Assignments.$.Note", note);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? "Ok" : "error";
        }

        public static string DeleteAssignment(string projectId, string id)
        {
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(projectId)), Query.EQ("Assignments._id", ObjectId.Parse(id))});
            var update = Update.Set("Assignments.$.IsDeleted", true);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? "Ok" : "error";
        }

        #endregion


        public static List<Project> GetAllProjects()
        {
            var query = Query.EQ("IsDeleted",false);

            return Coll.Find(query).ToList();
        }

        public static void Insert(string name, int priority, DateTime createdDate, string status, int noise, string product, string projectType)
        {
            Coll.Insert(new Project
                            {
                                CreatedDate = createdDate,
                                Name = name,
                                Priority = priority,
                                Status = status,
                                Noise = noise,
                                Product = ObjectId.Parse(product),
                                Type = projectType
                            });
        }

        public static void Delete(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("IsDeleted", true);

            Coll.Update(query, update);
        }

        public static void Save(string id, string name, int priority, string status, int noise, string product, string projectType)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", name).Set("Priority", priority).Set("Status", status).Set("Noise", noise).Set("Product", ObjectId.Parse(product)).Set("Type", projectType);

            Coll.Update(query, update);
        }
       
    }
}
