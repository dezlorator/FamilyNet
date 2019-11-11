using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    public class PurchaseFilterDTO
    {
        public IEnumerable<PurchaseDTO> PurchaseDTOs { get; set; }


        public int TotalCount { get; set; }
    }
}
