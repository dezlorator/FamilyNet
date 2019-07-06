using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class BaseItemType
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<BaseItemType> Childs { get; set; }
        public ICollection<BaseItemType> Parent { get; set; }
    }
}