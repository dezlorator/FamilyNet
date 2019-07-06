using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Representative : Person
    {
        [Required]
        public Orphanage Orphanage { get; set; }
    }
}