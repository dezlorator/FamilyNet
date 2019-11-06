using DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class AuctionLotFilterModel
    {       
        public string SelectedName { get;  set; }   

        public string StartPrice { get; set; }   

        public string EndPrice { get; set; }    
    }
}
