using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DnTeam.Models
{
    public class DepartmentModel
    {
        [ReadOnly(true)]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Parent Department")]
        public string ParentDepartment { get; set; }
        
        [UIHint("Locations")]
        public string Location { get; set; }

        [UIHint("Currency")]
        public decimal Cost { get; set; }

        [UIHint("Currency")]
        public decimal Rate { get; set; }

        //public string Edit
        //{
        //    get
        //    {
        //        return string.IsNullOrEmpty(Id) 
        //            ? string.Empty
        //            : string.Format("<a class=\"t-button t-button-icontext\" onclick=\"editDepartments('{0}','{1}','{2}','{3}','{4}','{5}')\"><span class=\"t-icon t-edit\"></span>Edit</a>",
        //            Id, Location, Name, ParentDepartment, Cost, Rate);
        //    }
        //}
    }
}