using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Orphanage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Adress Adress { get; set; }
        public float Rating { get; set; }
        public string Avatar { get; set; }

        public ICollection<Representative> Representatives { get; set; }
        public ICollection<Orphan> OrphansIds { get; set; }
    }
}
