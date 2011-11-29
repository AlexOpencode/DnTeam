using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DnTeam.Models
{
    public class EditableClientModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class EditableProductModel
    {
        public string Id { get; set; }

        [Required]
        [DisplayName("Product Name")]
        public string Name { get; set; }

        [UIHint("Clients"), Required]
        [DisplayName("Client Name")]
        public string  Client { get; set; }
        
    }
}