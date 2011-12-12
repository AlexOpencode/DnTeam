using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DnTeamData;
using DnTeamData.Models;


namespace DnTeam.Models
{
 
    public class PersonModel
    {
        [ReadOnly(true)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [UIHint("Locations")]
        [Display(Name = "Location")]
        public string LocatedIn { get; set; }
        
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [UIHint("Persons")]
        [Display(Name = "Primary manager")]
        public string PrimaryManager { get; set; }

        [Display(Name = "Other managers")]
        public IEnumerable<KeyValuePair<string,string>> OtherManagers { get; set; }

        [UIHint("Persons")]
        [Display(Name = "Primary peer")]
        public string PrimaryPeer { get; set; }

        [Display(Name = "Other peers")]
        public IEnumerable<KeyValuePair<string, string>> OtherPeers { get; set; }

        [Display(Name = "Likes to work with")]
        public IEnumerable<KeyValuePair<string, string>> LikesToWorkWith { get; set; }
    
        [Display(Name = "Direct Reports")]
        public IEnumerable<KeyValuePair<string, string>> DirectReports { get; set; }

        [Display(Name = "Links")]
        public List<string> Links { get; set; }
        
        [UIHint("NullableDate")]
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DoB { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Photo URL")]
        public string PhotoUrl { get; set; }

        [Display(Name = "OpenId (user will be able to Log in the system)")]
        public string OpenId { get; set; }

        public string PrimaryManagerName { get; set; }
        public string PrimaryPeerName { get; set; }
        public string DepartmentDescription { get; set; }

        public IEnumerable<Specialty> GetTechnologySpecialties()
        {
            return PersonRepository.GetTechnologySpecialties(Id);
        }

        public IEnumerable<Specialty> GetProjectSpecialties()
        {
            return ProjectRepository.GetPersonProjectSpecialties(Id);
        }
       
    }

    public class PersonGridModel
    {
        public string Id { get; set; }

        [Display(Name = "Name"), Required]
        public string Name { get; set; }

        [UIHint("Locations")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [UIHint("Persons")]
        [Display(Name = "Primary manager")]
        public string PrimaryManager { get; set; }

        [Display(Name = "Technology skills"), ReadOnly(true)]
        public string TechnologySkills { get; set; }
    }

    public class SpecialtyModel
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
        /// Specialty fist used date
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime FirstUsed { get; set; }

        /// <summary>
        /// Specialty last used date
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Some notes to last project
        /// </summary>
        public string LastProjectNote { get; set; }
    }
}
