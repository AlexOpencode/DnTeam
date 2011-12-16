using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;

namespace DnTeamData.Models
{
    /// <summary>
    /// Class describes Project object
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Project Id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        /// <summary>
        /// Parent Product Id
        /// </summary>
        public ObjectId ProductId { get; set; }
        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName
        {
            get { return (ProductId == ObjectId.Empty) ? string.Empty : ProductRepository.GetName(ProductId); }
        }
        /// <summary>
        /// Defines whether project is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Project Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Project Status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Project Priority
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// Project Type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Creation time stamp
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Project start date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Project end date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Project noise type
        /// </summary>
        public string Noise { get; set; }
        /// <summary>
        /// The list of project milestones
        /// </summary>
        public List<Milestone> Milestones { get; set; }
        /// <summary>
        /// The list of project assignments
        /// </summary>
        public List<Assignment> Assignments { get; set; }
        /// <summary>
        /// Project constructor assigns Id to new project
        /// </summary>
        public Project()
        {
            Id = ObjectId.GenerateNewId();
        }
        /// <summary>
        /// Returns Program Manager name
        /// </summary>
        /// <returns>Program Manager name</returns>
        public string ProgramManagerName()
        {
            var assignments = Assignments.Where(o => o.Role == "Program Manager").ToList();
            return (assignments.Count() <= 0) ? "wanted" : assignments.First().PersonName;
        }
        /// <summary>
        /// Returns Technical Lead name
        /// </summary>
        /// <returns>Technical Lead name</returns>
        public string TechnicalLeadName()
        {
            var assignments = Assignments.Where(o => o.Role == "Technical Lead").ToList();
            return (assignments.Count() <= 0) ? "wanted" : assignments.First().PersonName;

        }
        /// <summary>
        /// Overrides ToString to return string value of project Id
        /// </summary>
        /// <returns>String value of project Id</returns>
        public override string ToString()
        {
            return Id.ToString();
        }
    }

    /// <summary>
    /// Class describes Assignment object
    /// </summary>
    public class Assignment
    {
        /// <summary>
        /// Assignment Id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        /// <summary>
        /// Assignment created time stamp
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// Defines whether Assignment is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Id of person assigned
        /// </summary>
        public ObjectId PersonId { get; set; }
        /// <summary>
        /// Name of person assigned
        /// </summary>
        public string PersonName
        {
            get { return (PersonId == ObjectId.Empty) ? "wanted" : PersonRepository.GetPersonName(PersonId); }
        }
        /// <summary>
        /// Project role
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// Assignment start date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Assignment end date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Person commitment persantage
        /// </summary>
        public int Commitment { get; set; }
        /// <summary>
        /// Assignment notes
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Assignment constructor: defines created timestamp, and assigns new id
        /// </summary>
        public Assignment()
        {
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
            CreatedTime = DateTime.Now;
            Id = ObjectId.GenerateNewId();
        }
        /// <summary>
        /// Overrides ToString method to return assignment id
        /// </summary>
        /// <returns>Assignment id</returns>
        public override string ToString()
        {
            return Id.ToString();
        }

    }

    /// <summary>
    /// Class describes Milestone object
    /// </summary>
    public class Milestone
    {
        /// <summary>
        /// Milstone Id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        /// <summary>
        /// Milestone name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Milestone index position
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Milestone target date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? TargetDate { get; set; }
        /// <summary>
        /// Milestone actial date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ActualDate { get; set; }
        /// <summary>
        /// Milestone Constructor: assigns new id
        /// </summary>
        public Milestone()
        {
            Id = ObjectId.GenerateNewId();
        }
        /// <summary>
        /// Overrides ToString method to return milestone id
        /// </summary>
        /// <returns>Milestone id</returns>
        public override string ToString()
        {
            return Id.ToString();
        }
    }

    /// <summary>
    /// Describes project update status
    /// </summary>
    public enum ProjectEditStatus
    {
        /// <summary>
        /// Project properties have been updated successfuly
        /// </summary>
        Ok,
        /// <summary>
        /// Project name is empty
        /// </summary>
        ErrorNameIsEmpty,
        /// <summary>
        /// Project already exists
        /// </summary>
        ErrorDuplicateItem,
        /// <summary>
        /// Property format is invalid
        /// </summary>
        ErrorUndefinedFormat,
        /// <summary>
        /// Property has not been updated
        /// </summary>
        ErrorPropertyHasNotBeenUpdated,
    }

    /// <summary>
    /// Describes milestone update status
    /// </summary>
    public enum MilestoneEditStatus
    {
        /// <summary>
        /// Project milestone has been updated successfuly
        /// </summary>
        Ok,
        /// <summary>
        /// Name of milestone is invalid
        /// </summary>
        ErrorNameIsEmpty,
        /// <summary>
        /// Name of milestone is duplicate
        /// </summary>
        ErrorDuplicateName,
        /// <summary>
        /// Target date format is invalid
        /// </summary>
        ErrorTargetDateFormat,
        /// <summary>
        /// Actual date format is invalid
        /// </summary>
        ErrorActualDateFormat,
        /// <summary>
        /// Milestone has not been updated
        /// </summary>
        ErrorMilestoneHasNotBeenUpdated
    }

    /// <summary>
    /// Describes assignment update status
    /// </summary>
    public enum AssignmentEditStatus
    {
        /// <summary>
        /// Project assignment has been updated successfuly
        /// </summary>
        Ok,
        /// <summary>
        /// Project assignment has not been inserted
        /// </summary>
        ErrorAssignmentHasNotBeenInserted,
        /// <summary>
        /// Project assignment has not been updated
        /// </summary>
        ErrorAssignmentHasNotBeenUpdated,
    }
}
