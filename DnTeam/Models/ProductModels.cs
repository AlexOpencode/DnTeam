using System.ComponentModel.DataAnnotations;

namespace DnTeam.Models
{
    public class ProductModel
    {
        public string Id { get; set; }

        [Required]
        [LocalizedDisplay("Products_Name")]
        public string Name { get; set; }

        [LocalizedDisplay("Products_Client")]
        public string  Client { get; set; }
    }
}