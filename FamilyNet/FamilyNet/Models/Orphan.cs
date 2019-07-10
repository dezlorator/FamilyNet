using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Orphan : Person
    {
        [Required]
        public virtual Orphanage Orphanage { get; set; }
        public virtual ICollection<AuctionLot> AuctionLots { get; set; }
    }
}