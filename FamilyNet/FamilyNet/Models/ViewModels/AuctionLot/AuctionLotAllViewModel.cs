using FamilyNet.Models.ViewModels.AuctionLot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class AuctionLotAllViewModel
    {
        [Required(ErrorMessage = "Please")]
        public IEnumerable<FamilyNet.Models.AuctionLot> AuctionLots { get; set; }

        public AuctionLotPageViewModel PageViewModel { get; set; }
        public AuctionLotFilterModel FilterViewModel { get; set; }

        public string Sort { get; set; }
    }
}
