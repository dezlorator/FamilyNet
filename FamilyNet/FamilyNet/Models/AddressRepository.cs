using FamilyNet.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class AddressRepository
    {
        private List<string> _countries = new List<string> { "Ukrain", "Russia", "Belarus" };
        private IEnumerable<Address> _allDataRows = null; //(((((
        private string _filePath = null;

        private Address _address = null;

        public AddressRepository(ApplicationDbContext context)
        {
        }

        public string this[int index]
        {
            get => _countries[index];
        }


        public IEnumerable<string> GetCountries()
        {
            return _countries;
        }

        public IEnumerable<string> GetRegion(string country)
        {
            IEnumerable<string> regions = null;
            _address.Country = country;

            int index = _countries.IndexOf(_address.Country);

            if (index != -1)
            {
                _filePath = FilePath(index);

                IEnumerable<Address> _allDataRows = from line in File.ReadLines(_filePath, Encoding.UTF8)
                                                    let data = line.Split(";")
                                                    select new Address
                                                    {
                                                        Region = data[0],
                                                        City = data[2],
                                                        Street = data[4],
                                                        House = data[5]
                                                    };

                regions = (from line in _allDataRows
                           select line.Region).Distinct();
            }

            return regions;
        }


        public IEnumerable<string> GetCities(string region)
        {
            IEnumerable<string> cities = null;
            _address.Region = region;

            cities = (from line in _allDataRows
                      where line.Region == region
                      select line.City).Distinct();

            return cities;
        }

        public IEnumerable<string> GetStreets(string city)
        {
            IEnumerable<string> streets = null;
            _address.City = city;

            streets = (from line in _allDataRows
                       where line.City == city
                       select line.Street).Distinct();

            return streets;
        }

        public IEnumerable<string> GetHouses(string street)
        {
            IEnumerable<string> houses = null;
            _address.Street = street;

            houses = (IEnumerable<string>)from line in _allDataRows
                                          select line.Street.Split(",");

            return houses;
        }

        public Address GetFullAddress(string house)
        {
            _address.House = house;

            return _address;
        }



        private string FilePath(int countryIndex)
        {
            string slnFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            string path = String.Concat(slnFolderPath, Path.DirectorySeparatorChar, _countries[countryIndex], ".csv");

            return path;
        }
    }
}
