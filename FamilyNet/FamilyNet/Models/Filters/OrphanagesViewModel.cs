using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyNet.Models.Filters
{
    public class OrphanagesViewModel
    {
        public IEnumerable<Orphanage> Orphanages { get; set; }
        public IEnumerable<Adress> Adresses { get; set; }
        public string Name { get; set; }
    }
}
