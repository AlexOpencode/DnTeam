using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DnTeam.Models
{
    public class DepartmentModel
    {
        [ReadOnly(true)]
        public string Id { get; set; }

        [Required]
        [LocalizedDisplay("Department_Name")]
        public string Name { get; set; }

        [LocalizedDisplay("Department_Parent")]
        public string ParentDepartment { get; set; }
        
        [LocalizedDisplay("Department_Location")]
        public string Location { get; set; }

        [UIHint("Currency")]
        [LocalizedDisplay("Department_Cost")]
        public decimal Cost { get; set; }

        [UIHint("Currency")]
        [LocalizedDisplay("Department_Rate")]
        public decimal Rate { get; set; }
    }
}