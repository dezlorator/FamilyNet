using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    public class FilterParamentrsPurchaseDTO
    {
        public string Email { get; set; }

        public string CraftName { get; set; }

        public DateTime Date { get; set; }

        public int Page { get; set; }

        public int Rows { get; set; }

        public string Sort { get; set; }
    }
}
