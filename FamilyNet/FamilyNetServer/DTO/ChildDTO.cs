using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.DTO
{
    public class ChildDTO
    {
        public int ID { get; set; }

        public virtual FullName FullName { get; set; }
        public virtual DateTime Birthday { get; set; }
        public float Rating { get; set; }
        public IFormFile Avatar { get; set; }
        public int MyProperty { get; set; }
        public int OrphanageID { get; set; }
    }
}
