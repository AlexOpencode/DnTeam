using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DnTeam.Models
{
 
    public class PersonModel
    {
        [ReadOnly(true)]
        public string Id { get; set; }

        [Required]
        [LocalizedDisplay("People_Name")]
        public string Name { get; set; }
        
        [LocalizedDisplay("People_Phone")]
        public string Phone { get; set; }

        [Required]
        [LocalizedDisplay("People_Location")]
        public string LocatedIn { get; set; }
        
        [DataType(DataType.MultilineText)]
        [LocalizedDisplay("People_Comments")]
        public string Comments { get; set; }

        [LocalizedDisplay("People_Primary_Manager")]
        public string PrimaryManager { get; set; }

        [LocalizedDisplay("People_Other_Managers")]
        public IEnumerable<KeyValuePair<string,string>> OtherManagers { get; set; }

        [LocalizedDisplay("People_Primary_Peer")]
        public string PrimaryPeer { get; set; }

        [LocalizedDisplay("People_Other_Peers")]
        public IEnumerable<KeyValuePair<string, string>> OtherPeers { get; set; }

        [LocalizedDisplay("People_Likes_to_Work_With")]
        public IEnumerable<KeyValuePair<string, string>> LikesToWorkWith { get; set; }
    
        [LocalizedDisplay("People_Direct_Reports")]
        public IEnumerable<KeyValuePair<string, string>> DirectReports { get; set; }

        [LocalizedDisplay("People_Links")]
        public List<string> Links { get; set; }

        [LocalizedDisplay("People_DoB")]
        [DataType(DataType.Date)]
        public DateTime? DoB { get; set; }

        [DataType(DataType.ImageUrl)]
        [LocalizedDisplay("People_Photo_URL")]
        public string PhotoUrl { get; set; }

        [LocalizedDisplay("People_OpenId")]
        public string OpenId { get; set; }

        [LocalizedDisplay("People_Primary_Manager")]
        public string PrimaryManagerName { get; set; }

        [LocalizedDisplay("People_Primary_Peer")]
        public string PrimaryPeerName { get; set; }

        [LocalizedDisplay("People_Location")]
        public string DepartmentDescription { get; set; }
    }

    public class PersonGridModel
    {
        public string Id { get; set; }

        [Required]
        [LocalizedDisplay("People_Name")]
        public string Name { get; set; }

        [LocalizedDisplay("People_Location")]
        public string Location { get; set; }

        [LocalizedDisplay("People_Primary_Manager")]
        public string PrimaryManager { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplay("People_Specialties")]
        public string TechnologySkills { get; set; }
    }

    public class FunctionalSpecialtyModel
    {
        /// <summary>
        /// Specialty Name
        /// </summary>
        [LocalizedDisplay("Specialty_Project_Name")]
        public string Name { get; set; }

        /// <summary>
        /// Specialty fist used date
        /// </summary>
        [LocalizedDisplay("Specialty_From")]
        [DataType(DataType.Date)]
        public DateTime FirstUsed { get; set; }

        /// <summary>
        /// Specialty last used date
        /// </summary>
        [LocalizedDisplay("Specialty_To")]
        [DataType(DataType.Date)]
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Project Id
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Project Id
        /// </summary>
        [LocalizedDisplay("Specialty_Roles")]
        public string Roles { get; set; }
    }

    public class TechnologySpecialtyModel
    {
        /// <summary>
        /// Specialty Name
        /// </summary>
        [LocalizedDisplay("Specialty_Name")]
        public string Name { get; set; }

        /// <summary>
        /// Specialty fist used date
        /// </summary>
        [DataType(DataType.Date)]
        [LocalizedDisplay("Specialty_First_Used")]
        public DateTime FirstUsed { get; set; }

        /// <summary>
        /// Specialty last used date
        /// </summary>
        [DataType(DataType.Date)]
        [LocalizedDisplay("Specialty_Last_Used")]
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Specialty Level
        /// </summary>
        [LocalizedDisplay("Specialty_Level")]
        public string Level { get; set; }

        /// <summary>
        /// Some notes to last project
        /// </summary>
        [LocalizedDisplay("Specialty_Last_Project_Note")]
        public string LastProjectNote { get; set; }
    }
}
