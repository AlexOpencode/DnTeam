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

            return Coll.FindOne(query).Assignments.Where(o => o.IsDeleted == false).ToList();
        }

        public static string CreateAssignment(string projectId, DateTime startDate, DateTime endDate, string person, string role, int commitment, string note)
        {
            var query = Query.EQ("_id", ObjectId.Parse(projectId));
            var update = Update.AddToSet("Assignments", new Assignment
            {
                StartDate = startDate,
                EndDate = endDate,
                Role = role,
                PersonId = ObjectId.Parse(person),
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
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(projectId)), Query.EQ("Assignments._id", ObjectId.Parse(id)) });
            var update = Update.Set("Assignments.$.IsDeleted", true);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? "Ok" : "error";
        }

        #endregion

        #region Milestones

        public static List<Milestone> GetMilestones(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));

            return Coll.FindOne(query).Milestones.ToList();
        }

        public static void CreateMilestone(string projectId, int index, DateTime actualDate, DateTime targetDate, string name)
        {
            using (Db.Server.RequestStart(Db))
            {
                var query = Query.EQ("_id", ObjectId.Parse(projectId));
                var update = Update.AddToSet("Milestones", new Milestone
                {
                    Index = index,
                    ActualDate = actualDate,
                    TargetDate = targetDate,
                    Name = name
                }.ToBsonDocument());

                Coll.Update(query, update, SafeMode.True);
                
                //Add a new milestone with the <name>, if it is absent
                SettingsRepository.UpdateSetting(EnumName.ProjectMilestones, name);
            }
        }

        public static string UpdateMilestone(string projectId, string id, int index, DateTime actualDate, DateTime targetDate, string name)
        {
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(projectId)), Query.EQ("Milestones._id", ObjectId.Parse(id)) });
            var update = Update.Set("Milestones.$.Index", index).Set("Milestones.$.ActualDate", actualDate).Set("Milestones.$.TargetDate", targetDate).Set("Milestones.$.Name", name);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? "Ok" : "error";
        }

        public static string DeleteMilestone(string projectId, string id)
        {
            var update = Update.Pull("Milestones", Query.EQ("_id", ObjectId.Parse(id)));
            var res = Coll.Update(Query.EQ("_id", ObjectId.Parse(projectId)), update, SafeMode.True);
            
            return res.Ok ? "Ok" : "error";
        }

        #endregion

        public static List<Project> GetAllProjects()
        {
            var query = Query.EQ("IsDeleted", false);

            return Coll.Find(query).ToList();
        }

        public static void Insert(string name, string priority, DateTime createdDate, string status, string noise, string product, string projectType, string programManager, 
            string technicalLead)
        {
            var milestonesName = SettingsRepository.GetAllMilestones();

            var res = Coll.Insert(new Project
                            {
                                CreatedDate = createdDate,
                                Name = name,
                                Priority = priority,
                                Status = status,
                                Noise = noise,
                                Product = ObjectId.Parse(product),
                                Type = projectType,
                                Assignments = new List<Assignment>
                                                  {
                                                      new Assignment {Role = "Program Manager", PersonId = ObjectId.Parse(programManager)},
                                                      new Assignment {Role = "Technical Lead", PersonId = ObjectId.Parse(technicalLead)}
                                                  },
                                Milestones = (milestonesName.Count() > 0)
                                                ? milestonesName.Select(m => new Milestone
                                                                              {
                                                                                  Index = 0,
                                                                                  ActualDate = DateTime.Now,
                                                                                  TargetDate = DateTime.Now,
                                                                                  Name = m
                                                                              }).ToList()
                                                : new List<Milestone>()
                            }, SafeMode.True);
            //TODO: return user-friendly error
        }

        public static void Delete(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("IsDeleted", true);

            Coll.Update(query, update);
        }

        public static void Save(string id, string name, string priority, string status, string noise, string product, string projectType, string programManager, string technicalLead)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", name).Set("Priority", priority).Set("Status", status).Set("Noise", noise).Set("Product", ObjectId.Parse(product)).Set("Type", projectType);

            Coll.Update(query, update);
        }

        public static Project GetProject(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));

            return Coll.FindOne(query);
        }

        public static string UpdateProjectProperty(string id, string name, string value)
        {
            dynamic val;
            switch (name)
            {
               case "Product":
                    val = ObjectId.Parse(value);
                    break;
                    
                default:
                    val = value;
                    break;
            }

            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set(name, val);

            var res = Coll.Update(query, update, SafeMode.True);

            return res.Ok ? string.Empty : "error description"; //TODO: return user-friendly error
        }
    }
}
