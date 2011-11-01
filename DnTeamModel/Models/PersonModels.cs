using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    public class Person
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string PersonId
        {
            get { return Id.ToString(); }
        }
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string Location { get; set; }
        public DateTime DoB { get; set; }
        public List<Specialty> TechnologySpecialties { get; set; }
        public ObjectId PrimaryManager { get; set; }
        public string PrimaryManagerName
        {
            get
            {
                return (PrimaryManager != ObjectId.Empty)
                    ? PersonsRepository.GetUserName(PrimaryManager) : "none";
            }
        }
        public List<ObjectId> OtherManagers { get; set; }
        public ObjectId PrimaryPeer { get; set; }
        public List<ObjectId> OtherPeers { get; set; }
        public List<ObjectId> DirectReports { get; set; }
        public List<ObjectId> LikesToWorkWith { get; set; }
        public List<string> Links { get; set; }
        public ObjectId LocatedInSubsidary { get; set; }

        public Person()
        {
            Id = ObjectId.GenerateNewId();
            TechnologySpecialties = new List<Specialty>();
            OtherManagers = new List<ObjectId>();
            OtherPeers = new List<ObjectId>();
            DirectReports = new List<ObjectId>();
            LikesToWorkWith = new List<ObjectId>();
        }
    }

    public class Specialty
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public DateTime ExperienceSince { get; set; }
        public DateTime LastUsed { get; set; }
        public string LastProjectNote { get; set; }
    }

    public enum UserCreateStatus
    {
        Success,
        DuplicateUserName,
        DuplicateEmail,
        InvalidPassword,
        InvalidEmail,
        InvalidUserName,
        ProviderError,
        UserRejected,
        InvalidPrimaryManager
    }
}
