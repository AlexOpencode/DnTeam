using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;

namespace DnTeamData.Models
{
    public class Project
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ProjectId
        {
            get { return Id.ToString(); }
        }

        public ObjectId Product { get; set; }
        public string ProductName
        {
            get { return (Product == ObjectId.Empty) ? string.Empty : ProductRepository.GetName(Product); }
        }

        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Type { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndDate { get; set; }

        public string Noise { get; set; }
        public List<Milestone> Milestones { get; set; }
        public List<Assignment> Assignments { get; set; }

        public Project()
        {
            Id = ObjectId.GenerateNewId();
        }

        public string ProgramManagerName
        {
            get
            {
                var assignments = Assignments.Where(o => o.Role == "Program Manager").ToList();
                return (assignments.Count() > 0) ? PersonsRepository.GetPersonName(assignments.First().PersonId) : "wanted";
            }
        }

        public string TechnicalLeadName
        {
            get
            {
                var assignments = Assignments.Where(o => o.Role == "Technical Lead").ToList();
                return (assignments.Count() > 0) ? PersonsRepository.GetPersonName(assignments.First().PersonId) : "wanted";
            }
        }
    }
    
    public class Assignment
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string AssignmentId
        {
            get { return Id.ToString(); }
        }
        public DateTime CreatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public ObjectId PersonId { get; set; }
        public string PersonName
        {
            get { return PersonsRepository.GetPersonName(PersonId); }
        }
        public string Role { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndDate { get; set; }
        public int Commitment { get; set; }
        public string Note { get; set; }

        public Assignment()
        {
            CreatedTime = DateTime.Now;
            Id = ObjectId.GenerateNewId();
        }

        
    }

    public class Milestone
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string MilestoneId
        {
            get { return Id.ToString(); }
        }
        public string Name { get; set; }
        public int Index { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TargetDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ActualDate { get; set; }
        
        public Milestone()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
