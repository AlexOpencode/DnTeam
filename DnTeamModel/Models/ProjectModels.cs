using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
        public int Priority { get; set; }
        public string Type { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndDate { get; set; }

        public int Noise { get; set; }
        public List<Milestone> Milestones { get; set; }
        public List<Assignment> Assignments { get; set; }

        public Project()
        {
            Id = ObjectId.GenerateNewId();
            Milestones = new List<Milestone>();
            Assignments = new List<Assignment>();
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
        public ObjectId Person { get; set; }
        //public string PersonId
        //{
        //    get { return Person.ToString(); }
        //}
        public string PersonName
        {
            get { return PersonsRepository.GetPersonName(Person); }
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
        public string Name { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TargetDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ActualDate { get; set; }
        public bool Status
        {
            get { return ActualDate <= TargetDate; }
        }
    }
}
