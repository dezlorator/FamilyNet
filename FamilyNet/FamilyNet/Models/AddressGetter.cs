using FamilyNet.Models.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyNet.Models {


    public class AddressGetter : EFRepositoryAsync<Orphanage> {
        private static List<string> _countries = new List<string> { "Ukraine", "Россия", "Беларусь" };
        private IEnumerable<Address> _allAddresses = null;
        private Address _address = null;
        private string _country = null;
        private static string _filePath = null;

        public string Country { get => _country; }
        public Address Address { get => _address; }

        public AddressGetter(ApplicationDbContext dbContext) : base(dbContext) {

        }
        //public AddressGetter(string country, IEnumerable<Address> addresses) {
        //    _country = country;
        //    _allAddresses = addresses;
        //}

        public IEnumerable<CatalogCountry> GetCountries() 
        {
            return _dbContext.CatalogCountries.AsEnumerable();
        }

        public IEnumerable<CatalogRegion> GetRegions(int countryID) 
            {
            return _dbContext.CatalogRegions.Where(c => c.CountryID == countryID).AsEnumerable();
        }

        public static IEnumerable<Address> GetAdresses(string country) {
            IEnumerable<Address> result = null;

            int index = _countries.IndexOf(country);

            if (index != -1) {
                _filePath = FilePath(index);

                result = from line in File.ReadLines(_filePath, Encoding.UTF8).Skip(1).Distinct()
                         let data = line.Split(";")
                         where data[5] != null && data[2] == "м. Дніпро"
                         select new Address {
                             Region = data[0],
                             City = data[2],
                             Street = data[4],
                             House = data[5]
                         };
            }



            return result;
        }

        public IEnumerable<string> GetRegion(string country) {
            //TODO: part to switch into current CultureInfo
            _address.Country = country;
            IEnumerable<string> regions = null;

            regions = (from line in _allAddresses
                       select line.Region).Distinct();

            return regions;
        }


        public IEnumerable<string> GetCities(string region) {
            IEnumerable<string> cities = null;
            Address.Region = region;

            cities = (from line in _allAddresses
                      where line.Region == region
                      select line.City).Distinct();

            return cities;
        }

        public IEnumerable<string> GetStreets(string city) {
            IEnumerable<string> streets = null;
            Address.City = city;

            streets = (from line in _allAddresses
                       where line.City == city
                       select line.Street).Distinct();

            return streets;
        }

        public IEnumerable<string> GetHouses(string street) {
            IEnumerable<string> houses = null;
            Address.Street = street;

            houses = (IEnumerable<string>)from line in _allAddresses
                                          select line.Street.Split(",");

            return houses;
        }

        public Address GetFullAddress(string house) {
            Address.House = house;

            return Address;
        }

        public void AddNewCountry(string newCountry) {
            string _country = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newCountry.ToLower());

            _countries.Add(_country);
        }

        public static IEnumerable<string> GetCountries() {
            return _countries;
        }

        private static string FilePath(int countryIndex) {
            //~\familynet\FamilyNet\FamilyNet folder
            string slnFolderPath = Directory.GetCurrentDirectory();

            string path = String.Concat(
                slnFolderPath, Path.DirectorySeparatorChar,
                "wwwroot", Path.DirectorySeparatorChar,
                "address", Path.DirectorySeparatorChar,
                _countries[countryIndex], ".csv");

            return path;
        }
    }
}