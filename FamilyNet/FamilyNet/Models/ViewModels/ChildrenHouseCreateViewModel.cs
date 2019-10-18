using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class ChildrenHouseCreateViewModel
    {
        public DataTransferObjects.ChildrenHouseDTO ChildrenHouse { get; set; }

        public DataTransferObjects.AddressDTO Address { get; set; }
    }
}
