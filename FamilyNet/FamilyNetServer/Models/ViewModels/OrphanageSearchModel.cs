﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyNetServer.Models.ViewModels
{
    public class OrphanageSearchModel
    {
        public string NameString { get; set; }

        public string AddressString { get; set; }

        public int RatingNumber { get; set; }
    }
}