using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    /// <summary>
    /// A class describing Person
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Person Id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        
        /// <summary>
        /// Person Name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Is person active
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Person phone number
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Comments about the person
        /// </summary>
        public string Comments { get; set; }
        
        /// <summary>
        /// Date of birth
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? DoB { get; set; }
        
        /// <summary>
        /// The list of technology specialties
        /// </summary>
        public List<Specialty> TechnologySpecialties { get; set; }
        
        /// <summary>
        /// Person's primary manager Id
        /// </summary>
        public ObjectId PrimaryManager { get; set; }
        
        /// <summary>
        /// Person's primary manager string Id value
        /// </summary>
        public string PrimaryManagerId
        {
            get { return PrimaryManager.ToString(); }
        }
        
        /// <summary>
        /// Person's primary manager name
        /// </summary>
        public string PrimaryManagerName
        {
            get
            {
                return (PrimaryManager != ObjectId.Empty) ? PersonRepository.GetPersonName(PrimaryManager) : "wanted";
            }
        }
        
        /// <summary>
        /// The list of person's managers
        /// </summary>
        public List<ObjectId> OtherManagers { get; set; }
        
        /// <summary>
        /// Person's primary peer Id
        /// </summary>
        public ObjectId PrimaryPeer { get; set; }
        
        /// <summary>
        /// Person's primary peer Id string value
        /// </summary>
        public string PrimaryPeerId
        {
            get { return PrimaryPeer.ToString(); }
        }

        /// <summary>
        /// Person's primary peer name
        /// </summary>
        public string PrimaryPeerName
        {
            get
            {
                return (PrimaryPeer != ObjectId.Empty) ? PersonRepository.GetPersonName(PrimaryPeer) : "wanted";
            }
        }

        /// <summary>
        /// Person's peers list
        /// </summary>
        public List<ObjectId> OtherPeers { get; set; }

        /// <summary>
        /// Person's direct reports list
        /// </summary>
        public List<ObjectId> DirectReports { get; set; }

        /// <summary>
        /// Person's likes to work with list
        /// </summary>
        public List<ObjectId> LikesToWorkWith { get; set; }

        /// <summary>
        /// Person's links list
        /// </summary>
        public List<string> Links { get; set; }

        /// <summary>
        /// Person's location id. Linked to SubsidaryId
        /// </summary>
        public ObjectId LocatedIn { get; set; }

        /// <summary>
        /// Person's location id string value
        /// </summary>
        public string LocationId
        {
            get { return LocatedIn.ToString(); }
        }

        /// <summary>
        /// Person's location name 
        /// </summary>
        public string LocationName
        {
            get { return (LocatedIn == ObjectId.Empty) ? "none" : DepartmentRepository.GetDepartmentName(LocatedIn); } 
        }
        
        /// <summary>
        /// Person's photo url
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// Person's OpenId account
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// Person's constructor. Defines default property values
        /// </summary>
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

        /// <summary>
        /// String list of person's managers
        /// </summary>
        public IEnumerable<string> OtherManagersList
        {
            get { return OtherManagers.Select(o => o.ToString()); }
        }

        /// <summary>
        /// String list of person's managers
        /// </summary>
        public IEnumerable<string> OtherPeersList
        {
            get { return OtherPeers.Select(o => o.ToString()); }
        }

        /// <summary>
        /// String list of person's direct reports
        /// </summary>
        public IEnumerable<string> DirectReportsList
        {
            get { return DirectReports.Select(o => o.ToString()); }
        }

        /// <summary>
        /// String list of person's colleagues, he likes to work with
        /// </summary>
        public IEnumerable<string> LikesToWorkWithList
        {
            get { return LikesToWorkWith.Select(o => o.ToString()); }
        }

        /// <summary>
        /// Override of ToString, returning person's Id
        /// </summary>
        public override string ToString()
        {
            return Id.ToString();
        }
    }

    /// <summary>
    /// Class describe Person's Specialty
    /// </summary>
    public class Specialty : IEquatable<Specialty>
    {
        /// <summary>
        /// Specialty Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specialty Level
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Specialty fist used
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FirstUsed { get; set; }

        /// <summary>
        /// Specialty LastUsed
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Notes to the last project
        /// </summary>
        public string LastProjectNote { get; set; }

        /// <summary>
        /// Implements IEquatable interface
        /// </summary>
        public bool Equals(Specialty other)
        {
            return Name == other.Name && Level == other.Level && FirstUsed == other.FirstUsed &&
                   LastUsed == other.LastUsed && LastProjectNote == other.LastProjectNote;
        }
    }

    /// <summary>
    /// Decribes Functional (project) specialties
    /// </summary>
    public class FunctionalSpecialty : Specialty
    {
        /// <summary>
        /// Project Id
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// User Roles
        /// </summary>
        public string Roles { get; set; }
    }

    /// <summary>
    /// The list of Person edit statuses
    /// </summary>
    public enum PersonEditStatus
    {
        /// <summary>
        /// Person properties have been updated successfuly
        /// </summary>
        Ok,
        /// <summary>
        /// Person with such name already exists
        /// </summary>
        ErrorDuplicateName,
        /// <summary>
        /// Person with such email already exists
        /// </summary>
        ErrorDuplicateEmail,
        /// <summary>
        /// Primary manager value is invalid
        /// </summary>
        ErrorInvalidPrimaryManager,
        /// <summary>
        /// Department location value is invalid
        /// </summary>
        ErrorInvalidLocation,
        /// <summary>
        /// Specilaty with such name already exists for this person
        /// </summary>
        ErrorDuplicateSpecialtyName,
        /// <summary>
        /// Property has not been updated
        /// </summary>
        ErrorPropertyHasNotBeenUpdated,
        /// <summary>
        /// Property has not been added
        /// </summary>
        ErrorPropertyHasNotBeenAdded,
        /// <summary>
        /// Duplicate property value
        /// </summary>
        ErrorDuplicateItem,
        /// <summary>
        /// Date format is not valid
        /// </summary>
        ErrorDateIsNotValid,
        /// <summary>
        /// Value's format is not valid
        /// </summary>
        ErrorUndefinedFormat,
        /// <summary>
        /// Error occured but it is undefined
        /// </summary>
        ErrorUndefined
    }
}
