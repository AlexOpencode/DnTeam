using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using DnTeam.Attributes;
using DnTeamData;
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

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime DoB { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }

        public SelectList LocationsList { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        
        [Display(Name = "Primary manager")]
        public string PrimaryManager { get; set; }

        public SelectList UsersList { get; set; }

        //[Required]
        //[Display(Name = "Technology skills")]
        //[TechnologySkills(ErrorMessage = "At least one skill shoul be more then 0")]
        //public List<Specialty> TechnologySkills { get; set; }

        public RegisterModel()
        {
          //  TechnologySkills = SettingsRepository.GetAllTechnologies().Select(o => new Specialty { Name = o, Value = 0 }).ToList(); 
            UsersList = new SelectList(UsersRepository.GetUsersList(), "key", "value");
            LocationsList = new SelectList(SettingsRepository.GetAllLocations());
        }
    }

    public class UsersGridModel
    {
        public string UserId { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Primary manager")]
        public string PrimaryManager { get; set; }

        [Display(Name = "Technology skills")]
        public string TechnologySkills { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }
    }
}
