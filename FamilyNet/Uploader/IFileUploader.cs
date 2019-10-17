using Microsoft.AspNetCore.Http;
using System.IO;

namespace Uploader
{
    public interface IFileUploader
    {
        string CopyFileToServer(string fileName, string directory, IFormFile file);
        Stream CopyFileToStream(IFormFile file);
    }
}
