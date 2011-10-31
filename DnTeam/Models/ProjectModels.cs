using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        //[ReadOnly(true)]
        //[DisplayName("Staff")]
        //public string HumanResources { get; set; }

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

    //public class CreateProjectModel
    //{
        
    //    [Required]
    //    [DisplayName("Project Name")]
    //    public string Name { get; set; }

    //    [Required]
    //    [DisplayName("Project Status")]
    //    public string ProjectStatus { get; set; }

    //    public List<string> ProjectStatuses { get; set; }

    //    [DisplayName("Priority")]
    //    public int Priority { get; set; }
    //}
}