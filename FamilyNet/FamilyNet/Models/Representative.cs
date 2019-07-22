using FamilyNet.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Representative : Person
    {
        public int OrphanageID { get; set; }
        //[Required]
        public virtual Orphanage Orphanage { get; set; }

    }
}