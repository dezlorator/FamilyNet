using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{
    public class ChildActivityRepository : EFRepository<ChildActivity>
    {
        #region fields

        private ApplicationDbContext _context;

        #endregion

        public ChildActivityRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
