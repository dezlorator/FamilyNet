using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models {
    public class CatalogCountry {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CatalogRegion> CatalogRegions { get; set; }
    }

    public class CatalogRegion {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CountryID { get; set; }
        public virtual CatalogCountry Country { get; set; }

        public virtual ICollection<CatalogCity> CatalogCities {
            get; set;
        }
    }

    public class CatalogCity {
        public int Id { get; set; }
        public string Name { get; set; }

        public int RegionID { get; set; }
        public virtual CatalogRegion Region { get; set; }
    }
}
