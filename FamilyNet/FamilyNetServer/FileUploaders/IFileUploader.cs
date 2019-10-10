using Microsoft.AspNetCore.Http;

namespace FamilyNetServer.FileUploaders
{
    public interface IFileUploader
    {
        string CopyFile(string fileName, string directory, IFormFile image);
    }
}
