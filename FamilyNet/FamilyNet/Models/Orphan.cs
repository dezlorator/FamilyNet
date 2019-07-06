using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Orphan : Person
    {
        [Required]
        public Orphanage Orphanage { get; set; }
        public ICollection<AuctionLot> AuctionLots { get; set; }
    }
}