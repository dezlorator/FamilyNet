﻿using FamilyNetServer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{
    public class ChildrenActivityRepository : EFRepository<ChildrenActivity>
    {
        #region fields

        private ApplicationDbContext _context;

        #endregion

        public ChildrenActivityRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
