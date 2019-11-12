using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    public class FilterParamentrsPurchaseDTO
    {
        public string UserId { get; set; }

        public int CraftId { get; set; }

        public DateTime Date { get; set; }

        public int QuantityFrom { get; set; }

        public int QuantityTo { get; set; }

        public float PaidFrom { get; set; }

        public float PaidTo { get; set; }

        public int Page { get; set; }

        public int Rows { get; set; }

        public string Sort { get; set; }
    }
}
