using System.ComponentModel.DataAnnotations;

namespace DnTeam.Models
{
    public class ClientModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}