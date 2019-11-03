using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLAvailabilitiesBuilder
    {
        string CreatePost(string api);
    }
}
