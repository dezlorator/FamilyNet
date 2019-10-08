using FamilyNetServer.Models.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyNetServer.Models
{
    public interface IGetAllAddresses
    {
        IEnumerable<Address> GetAdresses(string country);
    }


    public class AddressGetter : IGetAllAddresses
    {
        private static List<string> _countries = new List<string> { "Ukraine", "Russia", " Belarus" };
        private IEnumerable<Address> _allDataRows = null;
        private Address _address = null;
        private string _country = null;
        private string _filePath = null;

        public string Country { get => _country; }
        public Address Address { get => _address; }

        public AddressGetter(string country)
        {
            _country = country;
            _allDataRows = GetAdresses(_country);
        }

        public IEnumerable<Address> GetAdresses(string country)
        {
            IEnumerable<Address> result = null;

            int index = _countries.IndexOf(Address.Country);

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
            }

            return result;
        }

        public IEnumerable<string> GetRegion(string country, IEnumerable<Address> addresses)
        {
            //TODO: part to switch into current CultureInfo

            IEnumerable<string> regions = null;
            Address.Country = country;

            regions = (from line in addresses
                       select line.Region).Distinct();

            return regions;
        }


        public IEnumerable<string> GetCities(string region, IEnumerable<Address> addresses)
        {
            IEnumerable<string> cities = null;
            Address.Region = region;

            cities = (from line in addresses
                      where line.Region == region
                      select line.City).Distinct();

            return cities;
        }

        public IEnumerable<string> GetStreets(string city, IEnumerable<Address> addresses)
        {
            IEnumerable<string> streets = null;
            Address.City = city;

            streets = (from line in addresses
                       where line.City == city
                       select line.Street).Distinct();

            return streets;
        }

        public IEnumerable<string> GetHouses(string street, IEnumerable<Address> addresses)
        {
            IEnumerable<string> houses = null;
            Address.Street = street;

            houses = (IEnumerable<string>)from line in addresses
                                          select line.Street.Split(",");

            return houses;
        }

        public Address GetFullAddress(string house)
        {
            Address.House = house;

            return Address;
        }

        public void AddNewCountry(string newCountry)
        {
            string _country = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newCountry.ToLower());

            _countries.Add(_country);
        }

        public static IEnumerable<string> GetCountries()
        {
            return _countries;
        }

        private string FilePath(int countryIndex)
        {
            //~\FamilyNetServer\FamilyNetServer\FamilyNetServer folder
            string slnFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            string path = String.Concat(
                slnFolderPath, Path.DirectorySeparatorChar,
                "wwwroot", Path.DirectorySeparatorChar,
                "address", Path.DirectorySeparatorChar,
                _countries[countryIndex], ".csv");

            return path;
        }
    }
}