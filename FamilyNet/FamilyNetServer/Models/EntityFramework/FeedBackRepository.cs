using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{
    public class FeedBackRepository : EFRepository<Feedback>
    {
        public FeedBackRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
