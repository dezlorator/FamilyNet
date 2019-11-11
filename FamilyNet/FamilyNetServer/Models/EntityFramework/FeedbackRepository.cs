using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{
    public class FeedbackRepository : EFRepository<Feedback>
    {
        public FeedbackRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
