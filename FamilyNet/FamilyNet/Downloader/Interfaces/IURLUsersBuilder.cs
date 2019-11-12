using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLUsersBuilder
    {
        string GetAll(string api);
        string GetById(string api, string id);
        string CreatePost(string api);
    }
}
