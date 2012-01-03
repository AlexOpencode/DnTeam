using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DnTeam.Models
{
    public class ProjectGridModel : ProjectModel
    {
        [UIHint("Persons")]
        [LocalizedDisplay("Projects_Program_Manager")]
        public string ProgramManager { get; set; }

        [UIHint("Persons")]
        [LocalizedDisplay("Projects_Technical_Lead")]
        public string TechnicalLead { get; set; }
    }

    public class ProjectModel
    {
        [ScaffoldColumn(true)]
        public string Id { get; set; }

        [Required]
        [LocalizedDisplay("Projects_Name")]
        public string Name { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplay("Projects_Created_Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [UIHint("ProjectStatuses")]
        [LocalizedDisplay("Projects_Status")]
        public string Status { get; set; }

        [Required]
        [UIHint("ProjectTypes")]
        [LocalizedDisplay("Projects_Type")]
        public string Type { get; set; }

        [UIHint("ProjectNoiseTypes")]
        [LocalizedDisplay("Projects_Noise")]
        public string Noise { get; set; }

        [Required]
        [UIHint("ProjectPriorityTypes")]
        [LocalizedDisplay("Projects_Priority")]
        public string Priority { get; set; }

        [Required]
        [UIHint("Products")]
        [LocalizedDisplay("Projects_ProductId")]
        public string ProductId { get; set; }

        public ProjectModel()
        {
            CreatedDate = DateTime.Now;
        }

        public bool IsDeleted { get; set; }
    }

    public class AssignmentModel
    {

        [LocalizedDisplay("Assignment_Role")]
        public string Role { get; set; }

        [LocalizedDisplay("Assignment_Person")]
        public string Person { get; set; }

        [LocalizedDisplay("Assignment_Note")]
        public string Note { get; set; }
        
        [LocalizedDisplay("Assignment_Commitment")]
        public int Commitment { get; set; }

        [LocalizedDisplay("Assignment_Start_Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [LocalizedDisplay("Assignment_End_Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [ReadOnly(true)]
        public string AssignmentId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplay("Assignment_Location")]
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

        [LocalizedDisplay("Milestone_Index")]
        public int Index { get; set; }

        [LocalizedDisplay("Milestone_Name")]
        public string Name { get; set; }

        [LocalizedDisplay("Milestone_Target_Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; set; }

        [LocalizedDisplay("Milestone_Actual_Date")]
        [DataType(DataType.Date)]
        public DateTime? ActualDate { get; set; }

        [LocalizedDisplay("Milestone_Status")]
        [ReadOnly(true)]
        public string Status
        {
            get
            {
                return (TargetDate == null || ActualDate == null)
                           ? string.Empty
                           : (ActualDate <= TargetDate)
                                 ? string.Format("<img alt=\"{0}\" src=\"{1}\"/>", Resources.Labels.Milestone_Status_Ok, VirtualPathUtility.ToAbsolute("~/Content/yes.png"))
                                 : string.Format("<img alt=\"{0}\" src=\"{1}\"/>", Resources.Labels.Milestone_Status_No, VirtualPathUtility.ToAbsolute("~/Content/no.png"));

            }
        }
    }
}