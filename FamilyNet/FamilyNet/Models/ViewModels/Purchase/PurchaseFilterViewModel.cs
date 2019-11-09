using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels.Purchase
{
    public class PurchaseFilterViewModel
    {
        public string UserId { get; set; }

        public int CraftId { get; set; }

        public DateTime Date { get; set; }

        public int QuantityFrom { get; set; }

        public int QuantityTo { get; set; }

        public float PaidFrom { get; set; }

        public float PaidTo { get; set; }
    }

}
