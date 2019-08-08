﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class RepresentativeViewModel
    {
        public IEnumerable<Representative> Representatives { get; set; }
        public FilterModel FilterModel { get; set; }

        public RepresentativeViewModel()
        {
            Representatives = new List<Representative>();
            FilterModel = new FilterModel();
        }
    }
}
