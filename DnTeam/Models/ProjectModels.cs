using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DnTeam.Models
{
    public class ProjectGridModel : ProjectModel
    {
        [Display(Name = "Program Manager")]
        public string ProgramManager { get; set; }

        [Display(Name = "Technical  Lead")]
        public string TechnicalLead { get; set; }
    }

    public class ProjectModel
    {
        [ScaffoldColumn(true)]
        public string Id { get; set; }

        [Required]
        [DisplayName("Project Name")]
        public string Name { get; set; }

        [ReadOnly(true)]
        [DisplayName("Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [UIHint("ProjectStatuses")]
        [DisplayName("Project Status")]
        public string Status { get; set; }

        [Required]
        [UIHint("ProjectTypes")]
        [DisplayName("Project Type")]
        public string Type { get; set; }

        [DataType("ProjectNoiseTypes")]
        [DisplayName("Noise")]
        public string Noise { get; set; }

        [Required]
        [DataType("ProjectPriorityTypes")]
        [DisplayName("Priority")]
        public string Priority { get; set; }

        [Required]
        [UIHint("Products")]
        [DisplayName("Product Line")]
        public string ProductId { get; set; }

        public ProjectModel()
        {
            CreatedDate = DateTime.Now;
        }

        public bool IsDeleted { get; set; }
    }

    public class AssignmentModel
    {

        [UIHint("ProjectRoles")]
        [DisplayName("Role")]
        public string Role { get; set; }

        [UIHint("Persons")]
        [DisplayName("Person")]
        public string Person { get; set; }

        [DisplayName("Assignment")]
        public string Note { get; set; }

        [DisplayName("Commitment %")]
        [Range(1, 100)]
        [DataType("Integer")]
        public int Commitment { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [ReadOnly(true)]
        public string AssignmentId { get; set; }

        [ReadOnly(true)]
        [DisplayName("Location")]
        public string Location { get; set; }

        public AssignmentModel()
        {
            Commitment = 100;
        }
    }

    public class MilestoneModel
    {
        [ReadOnly(true)]
        public string MilestoneId { get; set; }

        [UIHint("Integer")]
        [DisplayName("Index")]
        public int Index { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Target Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; set; }

        [DisplayName("Actual Date")]
        [DataType(DataType.Date)]
        public DateTime? ActualDate { get; set; }

        [DisplayName("Status")]
        [ReadOnly(true)]
        public string Status
        {
            get
            {
                return (TargetDate == null || ActualDate == null)
                           ? string.Empty
                           : (ActualDate <= TargetDate)
                                 ? "<img alt=\"In time\" src=\"" + VirtualPathUtility.ToAbsolute("~/Content/yes.png") +
                                   "\"/>"
                                 : "<img alt=\"Not in time\" src=\"" + VirtualPathUtility.ToAbsolute("~/Content/no.png") +
                                   "\"/>";
            }
        }
    }
}