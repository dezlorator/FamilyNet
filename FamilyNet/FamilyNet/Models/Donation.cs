using System;

namespace FamilyNet.Models
{
    public class Donation
    {
        public int ID { get; set; }
        public DonationItem DonationItem { get; set; }
        public DateTime Date { get; set; }
    }
}