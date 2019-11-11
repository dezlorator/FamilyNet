using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    public class AuctionLotFilterDTO
    {
        public IEnumerable<AuctionLotDTO> AuctionLotDTOs { get; set; }

        public int TotalCount { get; set; }
    }
}
