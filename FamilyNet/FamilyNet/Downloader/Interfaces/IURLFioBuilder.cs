using DataTransferObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader.Interfaces
{
    public interface IURLFioBuilder
    {
        string GetById(string api, int id, UserRole role);
    }
}
