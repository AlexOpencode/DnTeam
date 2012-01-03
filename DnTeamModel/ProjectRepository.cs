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
    /// A static class that manupilates Project data
    /// </summary>
    public static class ProjectRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Project> _coll = Db.GetCollection<Project>("Projects");

        #if DEBUG //Test area
        /// <summary>
        /// Set the name of the collection
        /// </summary>
        /// <param name="collectionName">Collection name</param>
        public static void SetTestCollection(string collectionName)
        {
            _coll = Db.GetCollection<Project>(collectionName);
        }
        #endif

        #region Assignments
        /// <summary>
        /// Returns the list of project's assignments
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns>The list of project's assignments</returns>
        public static List<Assignment> GetAssignments(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));

            return _coll.FindOne(query).Assignments.Where(o => o.IsDeleted == false).ToList();
        }

        /// <summary>
        /// Inserts a new assignment to the defined project
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="startDate">Assignment start date</param>
        /// <param name="endDate">Assignment end date</param>
        /// <param name="person">Person Id to be assigned</param>
        /// <param name="role">Project Role</param>
        /// <param name="commitment">Person commitment %</param>
        /// <param name="note">Assignment note</param>
        /// <returns>Assignment edit status </returns>
        public static AssignmentEditStatus InsertAssignment(string id, string role, string person, string note, string startDate, string endDate, int commitment)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.AddToSet("Assignments", new Assignment
            {
                StartDate = DateTime.Parse(startDate).Date,
                EndDate = DateTime.Parse(endDate).Date,
                Role = role,
                PersonId = string.IsNullOrEmpty(person) ? ObjectId.Empty : ObjectId.Parse(person),
                Commitment = commitment,
                Note = note
            }.ToBsonDocument());

            var res = _coll.Update(query, update, SafeMode.True);

            return (res.DocumentsAffected > 0) ? AssignmentEditStatus.Ok : AssignmentEditStatus.ErrorNotInserted;
        }

        /// <summary>
        /// Updetes the defined assignment
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="assignmentId">Assignment Id</param>
        /// <param name="startDate">Assignment start date</param>
        /// <param name="endDate">Assignment end date</param>
        /// <param name="person">Person Id to be assigned</param>
        /// <param name="role">Project Role</param>
        /// <param name="commitment">Person commitment %</param>
        /// <param name="note">Assignment note</param>
        /// <returns>Assignment edit status </returns>
        public static AssignmentEditStatus UpdateAssignment(string id, string assignmentId, string role, string person, string note, string startDate, string endDate, int commitment)
        {
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(id)), Query.EQ("Assignments._id", ObjectId.Parse(assignmentId)) });
            var update = Update.Set("Assignments.$.StartDate", DateTime.Parse(startDate).Date).Set("Assignments.$.EndDate", DateTime.Parse(endDate).Date)
                .Set("Assignments.$.PersonId", string.IsNullOrEmpty(person) ? ObjectId.Empty : ObjectId.Parse(person)).Set("Assignments.$.Role", role)
                .Set("Assignments.$.Commitment", commitment).Set("Assignments.$.Note", note);

            var res = _coll.Update(query, update, SafeMode.True);

            return res.UpdatedExisting ? AssignmentEditStatus.Ok : AssignmentEditStatus.ErrorNotUpdated;
        }

        /// <summary>
        /// Deletes defined assignments
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="values">The list of assignments to delete</param>
        public static void DeleteAssignments(string projectId, IEnumerable<string> values)
        {
            //in case of IsDeleted
            //var proj = GetProject(projectId);
            //proj.Assignments.Where(o => values.Select(ObjectId.Parse).Contains(o.Id)).ToList().ForEach(o=>o.IsDeleted = true);
            //Coll.Save(proj.ToBsonDocument(), SafeMode.True);

            var update = Update.Pull("Assignments", Query.In("_id", new BsonArray(values.Select(ObjectId.Parse))));
            _coll.Update(Query.EQ("_id", ObjectId.Parse(projectId)), update, SafeMode.True);
        }

        #endregion

        #region Milestones

        /// <summary>
        /// Validates milestone properties and coverts them to proper format
        /// </summary>
        /// <param name="name">Milestone name</param>
        /// <param name="targetDate">Target date</param>
        /// <param name="actualDate">Actual date</param>
        /// <param name="errorTargetDateFormat">output ErrorTargetDateFormat</param>
        /// <param name="tDate">output DateTime Target date</param>
        /// <param name="aDate">output DateTime Actual date</param>
        /// <returns>Validation result. True - if properties are valid</returns>
        private static bool ValidateMilestone(string name, string targetDate, string actualDate,
          out MilestoneEditStatus errorTargetDateFormat, out DateTime? tDate, out DateTime? aDate)
        {
            tDate = null;
            aDate = null;
            errorTargetDateFormat = MilestoneEditStatus.Ok;

            if (string.IsNullOrEmpty(name))
            {
                errorTargetDateFormat = MilestoneEditStatus.ErrorNoName;
                return false;
            }

            //parse target date
            if (!string.IsNullOrEmpty(targetDate))
            {
                DateTime date;
                if (!DateTime.TryParse(targetDate, out date))
                {
                    errorTargetDateFormat = MilestoneEditStatus.ErrorTargetDateFormat;
                    return false;
                }

                tDate = date;
            }

            //parse actual date
            if (!string.IsNullOrEmpty(actualDate))
            {
                DateTime date;
                if (!DateTime.TryParse(actualDate, out date))
                {
                    errorTargetDateFormat = MilestoneEditStatus.ErrorActualDateFormat;
                    return false;
                }

                aDate = date;
            }

            return true;
        }

        /// <summary>
        /// Returns the list of milestones of the defined project
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>The list of milestones</returns>
        public static List<Milestone> GetMilestones(string id)
        {
            return GetProject(id).Milestones;
        }

        /// <summary>
        /// Creates a new milestone in the defined project
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="name">Milestone name</param>
        /// <param name="targetDate">Target date</param>
        /// <param name="actualDate">Actual date</param>
        /// <param name="index">Milestone index (default is 0)</param>
        /// <param name="addName">True - to add milestone name to settings. Default is true</param>
        /// <returns>Milestone edit status</returns>
        public static MilestoneEditStatus InsertMilestone(string projectId, string name, string targetDate, string actualDate, int index = 0, bool addName = true)
        {
            DateTime? tDate;
            DateTime? aDate;
            MilestoneEditStatus errorTargetDateFormat;
            if (!ValidateMilestone(name, targetDate, actualDate, out errorTargetDateFormat, out tDate, out aDate)) 
                return errorTargetDateFormat;

            using (Db.Server.RequestStart(Db))
            {
                var query = Query.And(new[] {Query.EQ("_id", ObjectId.Parse(projectId)), Query.NE("Milestones.Name", name)});
                var update = Update.AddToSet("Milestones", new Milestone
                {
                    Index = index,
                    ActualDate = aDate,
                    TargetDate = tDate,
                    Name = name
                }.ToBsonDocument());

                try
                {
                    var res = _coll.Update(query, update, SafeMode.True);
                    
                    //Add a new milestone with the <name>
                    if (addName) SettingsRepository.AddSettingValue(EnumName.ProjectMilestones, name);

                    return (res.DocumentsAffected == 0) ? MilestoneEditStatus.ErrorDuplicateName : MilestoneEditStatus.Ok;
                    
                }
                catch (MongoSafeModeException ex)
                {
                    if (ex.Message.Contains("duplicate")) return MilestoneEditStatus.ErrorDuplicateName;

                    throw;
                }
            }
        }
        
        /// <summary>
        /// Updates the defined milestone of the defined project
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="id">Milestone Id</param>
        /// <param name="targetDate">Target date</param>
        /// <param name="actualDate">Actual date</param>
        /// <param name="index">Milestone index (default is 0)</param>
        /// <returns>Milestone edit status</returns>
        public static MilestoneEditStatus UpdateMilestone(string projectId, string id, string targetDate, string actualDate, int index = 0)
        {
            DateTime? tDate;
            DateTime? aDate;
            MilestoneEditStatus errorTargetDateFormat;
            if (!ValidateMilestone("no name", targetDate, actualDate, out errorTargetDateFormat, out tDate, out aDate))
                return errorTargetDateFormat;

            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(projectId)), Query.EQ("Milestones._id", ObjectId.Parse(id)) });
            var update = Update.Set("Milestones.$.Index", index).Set("Milestones.$.ActualDate", aDate).Set("Milestones.$.TargetDate", tDate);

            try
            {
                var res = _coll.Update(query, update, SafeMode.True);

                return res.UpdatedExisting ? MilestoneEditStatus.Ok : MilestoneEditStatus.ErrorNotUpdated;
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return MilestoneEditStatus.ErrorDuplicateName;

                throw;
            }
        }

        /// <summary>
        /// Deletes defined molestones from the defined project
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="values">The list of milestone Ids</param>
        public static void DeleteMilestones(string projectId, List<string> values)
        {
            var update = Update.Pull("Milestones", Query.In("_id", new BsonArray(values.Select(ObjectId.Parse))));
            _coll.Update(Query.EQ("_id", ObjectId.Parse(projectId)), update, SafeMode.True);
        }

        /// <summary>
        /// Sets all selected milestones with emty ActualDate - current date
        /// </summary>
        /// <param name="projectId">Parent project id</param>
        /// <param name="values">The list of milestones</param>
        public static void FinishMilestones(string projectId, List<string> values)
        {
            var proj = GetProject(projectId);
            proj.Milestones.Where(o => values.Select(ObjectId.Parse).Contains(o.Id) && o.ActualDate == null).ToList().ForEach(o => o.ActualDate = DateTime.Now.Date);

            _coll.Save(proj.ToBsonDocument(), SafeMode.True);
        }

        #endregion

        /// <summary>
        /// Returns the list of all projects that are not deleted
        /// </summary>
        /// <returns>The list of all projects</returns>
        public static IEnumerable<Project> GetAllProjects(bool isDeleted = false)
        {
            var query = Query.EQ("IsDeleted", isDeleted);

            return _coll.Find(query);
        }

        /// <summary>
        /// Inserts a new project. Assignments for Program Manager and Technical Lead are also created.
        /// Also Milestones with all defined milestone names are created. 
        /// </summary>
        /// <param name="name">Project name</param>
        /// <param name="priority">Project priority</param>
        /// <param name="createdDate">Project created date</param>
        /// <param name="status">Project status</param>
        /// <param name="noise">Project noise type</param>
        /// <param name="product">Parent product id</param>
        /// <param name="projectType">Project type</param>
        /// <param name="programManager">Program Manager id</param>
        /// <param name="technicalLead">Technical lead id</param>
        /// <returns>Project edit status</returns>
        public static ProjectEditStatus Insert(string name, string priority, DateTime createdDate, string status, string noise, string product, string projectType,
            string programManager, string technicalLead)
        {
            var milestonesName = SettingsRepository.GetSettingValues(EnumName.ProjectMilestones);

            if (string.IsNullOrEmpty(name)) 
                return ProjectEditStatus.ErrorNoName;

            try
            {
                _coll.Insert(new Project
                                {
                                    CreatedDate = createdDate,
                                    Name = name,
                                    Priority = priority,
                                    Status = status,
                                    Noise = noise,
                                    ProductId = ObjectId.Parse(product),
                                    Type = projectType,
                                    Assignments = new List<Assignment>
                                                  {
                                                      new Assignment {Role = "Program Manager", PersonId = string.IsNullOrEmpty(programManager) 
                                                          ? ObjectId.Empty : ObjectId.Parse(programManager), Commitment = 100},
                                                      new Assignment {Role = "Technical Lead", PersonId = string.IsNullOrEmpty(technicalLead) 
                                                          ? ObjectId.Empty :  ObjectId.Parse(technicalLead), Commitment = 100}
                                                  },
                                    Milestones = (milestonesName.Count() > 0)
                                                    ? milestonesName.Select(m => new Milestone
                                                                                  {
                                                                                      Index = 0,
                                                                                      ActualDate = null,
                                                                                      TargetDate = null,
                                                                                      Name = m
                                                                                  }).ToList()
                                                    : new List<Milestone>()
                                }, SafeMode.True);
                
                return ProjectEditStatus.Ok;
            }
            catch(MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return ProjectEditStatus.ErrorDuplicateItem;

                throw;
            }
        }

        /// <summary>
        /// Deletes the list of defined projects
        /// </summary>
        /// <param name="values">The list of projects to delete</param>
        public static void Delete(IEnumerable<string> values)
        {
            var query = Query.In("_id", new BsonArray(values.Select(ObjectId.Parse)));
            var update = Update.Set("IsDeleted", true);

            _coll.Update(query, update, UpdateFlags.Multi);
        }

        /// <summary>
        /// Returns the defined project
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns>Project</returns>
        public static Project GetProject(string id)
        {
            return _coll.FindOneById(ObjectId.Parse(id));
        }

        /// <summary>
        /// Updates project property value
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        /// <returns>Project edit status</returns>
        public static ProjectEditStatus UpdateProjectProperty(string id, string name, string value)
        {
            dynamic val; 
            if(!Common.GetTypedPropertyValue(name, value, typeof(Project), out val))
                return ProjectEditStatus.ErrorUndefinedFormat;
            
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set(name, val ?? BsonNull.Value);
            try
            {
                var res = _coll.Update(query, update, SafeMode.True);
                return res.UpdatedExisting ? ProjectEditStatus.Ok : ProjectEditStatus.ErrorPropertyNotUpdated;
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return ProjectEditStatus.ErrorDuplicateItem;

                throw;
            }
        }

        /// <summary>
        /// Returns the list of projects person partisipated in
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <returns>The list of project specialties</returns>
        public static IEnumerable<FunctionalSpecialty> GetPersonProjectSpecialties(string id)
        {
            var personId = ObjectId.Parse(id);
            var query = Query.EQ("Assignments.PersonId", personId);
            var cursor = _coll.Find(query);

            return cursor.Select(o => new FunctionalSpecialty
                                            {
                                                Name = o.Name,
                                                ProjectId = o.Id.ToString(),
                                                FirstUsed = o.Assignments.Where(x => x.PersonId == personId).OrderBy(x => x.StartDate).First().StartDate,
                                                LastUsed = o.Assignments.Where(x => x.PersonId == personId).OrderByDescending(x => x.EndDate).First().EndDate,
                                                Roles = o.Assignments.Where(x => x.PersonId == personId).Select(x => x.Role).Distinct().OrderByDescending(x=>x)
                                                    .Aggregate((workingSentence, next) => next + ", " + workingSentence)
                                            });
        }

        /// <summary>
        /// Returns the list of product ids used by projects
        /// </summary>
        /// <returns>The list of product ids</returns>
        internal static IEnumerable<ObjectId> GetUsedProducts()
        {
            var cursor = _coll.FindAll();
            cursor.Fields = Fields.Include("Product");

            return cursor.Select(o => o.ProductId);
        }
    }
}
