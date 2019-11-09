using DataTransferObjects;
using FamilyNetServer.Models.Identity;
using System.Collections.Generic;

namespace FamilyNetServer.Factories
{
    public interface ITokenFactory
    {
        TokenDTO Create(ApplicationUser user, IList<string> roles);
    }
}
