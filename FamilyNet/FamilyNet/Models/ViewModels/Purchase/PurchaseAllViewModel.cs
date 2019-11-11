using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels.Purchase
{
    public class PurchaseAllViewModel
    {
        public IEnumerable<PurchaseDTO> PurchaseDTO { get; set; }

        public PageViewModel PageViewModel { get; set; }
        public PurchaseFilterViewModel FilterViewModel { get; set; }

        public string Sort { get; set; }
    }
}
