using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Comments { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? DoB { get; set; }
        public List<Specialty> TechnologySpecialties { get; set; }
        public ObjectId PrimaryManager { get; set; }
        public string PrimaryManagerId
        {
            get { return PrimaryManager.ToString(); }
        }
        public string PrimaryManagerName
        {
            get
            {
                return (PrimaryManager != ObjectId.Empty)
                    ? PersonsRepository.GetPersonName(PrimaryManager) : "wanted";
            }
        }
        public List<ObjectId> OtherManagers { get; set; }
        public ObjectId PrimaryPeer { get; set; }
        public string PrimaryPeerId
        {
            get { return PrimaryPeer.ToString(); }
        }
        public List<ObjectId> OtherPeers { get; set; }
        public List<ObjectId> DirectReports { get; set; }
        public List<ObjectId> LikesToWorkWith { get; set; }
        public List<string> Links { get; set; }
        public ObjectId LocatedIn { get; set; } //Linked to SubsidaryId
        public string LocationId
        {
            get { return LocatedIn.ToString(); }
        }
        public string LocationName
        {
            get { return (LocatedIn == ObjectId.Empty) ? "none" : "not implemented"; } //Todo: implement GetLocationName()
        }
        public string PhotoUrl { get; set; }
        public string OpenId { get; set; }

        public Person()
        {
            Id = ObjectId.GenerateNewId();
            TechnologySpecialties = new List<Specialty>();
            OtherManagers = new List<ObjectId>();
            OtherPeers = new List<ObjectId>();
            DirectReports = new List<ObjectId>();
            LikesToWorkWith = new List<ObjectId>();
            Links = new List<string>();
            PrimaryPeer = ObjectId.Empty;
            PrimaryManager = ObjectId.Empty;
            LocatedIn = ObjectId.Empty;
            IsActive = true;
        }

        public IEnumerable<string> OtherManagersList
        {
            get { return OtherManagers.Select(o => o.ToString()); }
        }

        public IEnumerable<string> OtherPeersList
        {
            get { return OtherPeers.Select(o => o.ToString()); }
        }

        public IEnumerable<string> DirectReportsList
        {
            get { return DirectReports.Select(o => o.ToString()); }
        }

        public IEnumerable<string> LikesToWorkWithList
        {
            get { return LikesToWorkWith.Select(o => o.ToString()); }
        }
    }

    public class Specialty
    {
        public string Name { get; set; }
        public int Level { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExperienceSince { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUsed { get; set; }
        public string LastProjectNote { get; set; }
    }

    public enum PersonCreateStatus
    {
        Success,
        DuplicateUserName,
        DuplicateEmail,
        InvalidPassword,
        InvalidEmail,
        InvalidUserName,
        ProviderError,
        UserRejected,
        InvalidPrimaryManager,
        InvalidLocation
    }
}
