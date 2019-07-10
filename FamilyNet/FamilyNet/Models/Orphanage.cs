using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Orphanage : IEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual Adress Adress { get; set; }
        public float Rating { get; set; }
        public string Avatar { get; set; }

        public virtual ICollection<Representative> Representatives { get; set; }
        public virtual ICollection<Orphan> OrphansIds { get; set; }
    }
}
