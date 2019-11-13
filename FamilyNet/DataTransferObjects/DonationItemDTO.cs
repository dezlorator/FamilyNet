using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObjects
{
    public class DonationItemDTO
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter desciption")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter price")]
        public float Price { get; set; }
        public IEnumerable<int> CategoriesID { get; set; }
    }
}
