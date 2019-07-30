using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyNet.Models.ViewModels
{
    public class OrphanagesViewModel
    {
        public IEnumerable<Orphanage> Orphanages { get; set; }

        public IEnumerable<Address> Adresses { get; set; }

        public string Name { get; set; }
    }
}
