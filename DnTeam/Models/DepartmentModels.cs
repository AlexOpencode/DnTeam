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
    }
}