using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLCategoriesBuilder
    {
        string GetAll(string api);

        string GetById(string api, int id);

        string CreatePost(string api);
    }
}
