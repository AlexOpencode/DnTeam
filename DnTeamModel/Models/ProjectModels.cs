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

        public string Name { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public string Type { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Nose { get; set; }
        public List<Milestone> Milestones { get; set; }
        public List<Assignment> Assignments { get; set; }

        public Project()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
    
    public class Assignment
    {
        [BsonId]
        public DateTime CreatedTime { get; set; }
        public ObjectId PersonId { get; set; }
        public string Role { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Commit { get; set; }
        public string FunctionalSpecialty { get; set; }

        Assignment()
        {
            CreatedTime = DateTime.Now;
        }
    }

    public class Milestone
    {
        public string Name { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime ActualDate { get; set; }
        public bool Status
        {
            get { return ActualDate <= TargetDate; }
        }
    }
}
