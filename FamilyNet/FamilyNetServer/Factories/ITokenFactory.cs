using FamilyNetServer.Models.Identity;
using System.Collections.Generic;

namespace FamilyNetServer.Factories
{
    public interface ITokenFactory
    {
        string Create(ApplicationUser user, IList<string> roles);
    }
}
