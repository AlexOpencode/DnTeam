using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DnTeam.Models
{
    public class ProductModel
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