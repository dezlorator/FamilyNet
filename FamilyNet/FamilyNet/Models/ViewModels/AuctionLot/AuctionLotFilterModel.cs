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

        public float StartPrice { get; set; }   

        public float EndPrice { get; set; }    
    }
}
