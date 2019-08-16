using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class TypeBaseItem
    {
        
        public int ItemID { get; set; }
        public virtual BaseItem Item { get; set; }

        public int TypeID { get; set; }
        public virtual BaseItemType Type { get; set; }
       
    }
}
