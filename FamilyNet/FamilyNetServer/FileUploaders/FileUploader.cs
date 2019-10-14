using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FamilyNetServer.FileUploaders
{
    public class FileUploader : IFileUploader
    {
        private readonly IHostingEnvironment _environment;

        public FileUploader(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public string CopyFile(string fileName, string directory, IFormFile file)
        {
            var webRoot = _environment.WebRootPath;
            var extension = Path.GetExtension(file.FileName);

            var filePath = Path.Combine(webRoot, directory, fileName) + extension;

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            return Path.Combine(directory, fileName).Replace("\\", "/") + extension;
        }
    }
}
