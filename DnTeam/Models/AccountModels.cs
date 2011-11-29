using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DnTeamData.Models;

namespace DnTeam.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class PersonModel
    {
        [ReadOnly(true)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [DataType(DataType.EmailAddress)]
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
    
        [Display(Name = "Technology Specialties")]
        public List<Specialty> TechnologySpecialties { get; set; }

        [Display(Name = "Direct Reports")]
        public IEnumerable<KeyValuePair<string, string>> DirectReports { get; set; }

        [Display(Name = "Links")]
        public List<string> Links { get; set; }
        
        [UIHint("NullableDate")]
        [Display(Name = "Date of birth")]
        public DateTime? DoB { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Photo URL")]
        public string PhotoUrl { get; set; }

        [Display(Name = "OpenId (user will be able to Log in the system)")]
        public string OpenId { get; set; }

        
    }

    public class PersonGridModel
    {
        public string Details
        {
            get { return string.IsNullOrEmpty(UserId) ? "" : "<a href=\"/Account/Details/" + UserId + "\"><img class=\"link-button\" src=\"../../Content/link.png\" alt=\"View\"/></a>"; }
        }
        public string UserId { get; set; }

        [Display(Name = "Name"), Required]
        public string UserName { get; set; }

        [UIHint("Locations")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [UIHint("Persons")]
        [Display(Name = "Primary manager")]
        public string PrimaryManager { get; set; }

        [Display(Name = "Technology skills"), ReadOnly(true)]
        public string TechnologySkills { get; set; }
    }
}
