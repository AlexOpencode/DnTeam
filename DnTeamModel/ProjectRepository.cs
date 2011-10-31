using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DnTeamData.Models;
using MongoDB.Driver;

namespace DnTeamData
{
    public static class ProjectRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();

        public static List<Project> GetAllProjects()
        {
            MongoCollection<Project> coll = Db.GetCollection<Project>("Projects");

            return coll.FindAll().ToList();
        }

        public static void Insert(string name, int priority, DateTime createdDate, string status)
        {
            MongoCollection<Project> coll = Db.GetCollection<Project>("Projects");
            coll.Insert(new Project()
                            {
                                CreatedDate = createdDate,
                                Name = name,
                                Priority = priority,
                                Status = status
                            });
        }
    }
}
