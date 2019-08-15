using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.AddressCatalog {
    public class CatalogCountry {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual List<CatalogRegion> Regions { get; set; }
    }

    public class CatalogRegion {
        public int ID { get; set; }
        public string Name { get; set; }

        public int CountryID { get; set; }
        public virtual CatalogCountry Country { get; set; }
        public virtual List<CatalogCities> Cities { get; set; }

    }

    public class CatalogCities {
        public int ID { get; set; }
        public string Name { get; set; }

        public int LocalityID { get; set; }
        public virtual CatalogRegion Region { get; set; }
    }

    public class ViewAddress 
        { 
        public int? Country { get; set; }
        public int? Region { get; set; }
        public int? City { get; set; }
        public int? Street { get; set; }
        public int? House { get; set; }


        public SelectList CountryList { get; set; }
        public SelectList RegionList { get; set; }
        public SelectList CityList { get; set; }
        public SelectList StreetList { get; set; }
    }
}
