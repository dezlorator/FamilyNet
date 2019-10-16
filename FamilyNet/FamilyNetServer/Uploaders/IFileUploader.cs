using Microsoft.AspNetCore.Http;
using System.IO;

namespace FamilyNetServer.Uploaders
{
    public interface IFileUploader
    {
        string CopyFileToServer(string fileName, string directory, IFormFile file);
    }
}
