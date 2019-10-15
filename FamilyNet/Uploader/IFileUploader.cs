using Microsoft.AspNetCore.Http;

namespace Uploader
{
    public interface IFileUploader
    {
        string CopyFileToServer(string fileName, string directory, IFormFile file);
        string CopyFileToClient(string fileName, string directory, IFormFile file);
    }
}
