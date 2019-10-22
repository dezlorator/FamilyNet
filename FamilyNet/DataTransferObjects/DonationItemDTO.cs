using System.Collections.Generic;

namespace DataTransferObjects
{
    public class DonationItemDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public IEnumerable<int> CategoriesID { get; set; }
    }
}
