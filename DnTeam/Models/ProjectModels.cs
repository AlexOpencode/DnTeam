using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DnTeam.Models
{
    public class ProjectEditableModel
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
        public string ProjectStatus { get; set; }

        [Required]
        [UIHint("ProjectTypes")]
        [DisplayName("Project Type")]
        public string ProjectType { get; set; }
        
        [DataType("Integer")]
        [DisplayName("Noise")]
        public int Noise { get; set; }

        [Required]
        [UIHint("Products")]
        [DisplayName("Product Line")]
        public string Product { get; set; }

        [Required]
        [DataType("Integer")]
        [DisplayName("Priority")]
        public int Priority { get; set; }

        public string Details
        {
            get { return string.IsNullOrEmpty(Id) ? "" : "<a href=\"/Project/Details/" + Id + "\"><img class=\"link-button\" src=\"../../Content/link.png\" alt=\"View\"/></a>"; }
        }

        public ProjectEditableModel()
        {
            CreatedDate = DateTime.Now;
        }
    }
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

    [DisplayName("Satart Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DisplayName("End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [ReadOnly(true)]
    public string AssignmentId { get; set; }

    public AssignmentModel()
    {
        Commitment = 100;
    }
}