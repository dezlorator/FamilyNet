using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Representative : Person
    {
        [Required]
        public virtual Orphanage Orphanage { get; set; }
    }
}