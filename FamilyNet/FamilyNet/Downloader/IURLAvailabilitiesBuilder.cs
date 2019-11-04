using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLAvailabilitiesBuilder
    {
        string GetAll(string api);
        string CreatePost(string api);
    }


}
