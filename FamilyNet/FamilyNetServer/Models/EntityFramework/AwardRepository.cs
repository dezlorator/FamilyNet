using FamilyNetServer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{
    public class AwardRepository : EFRepository<Award>
    {
        #region fields

        private ApplicationDbContext _context;

        #endregion

        public AwardRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
