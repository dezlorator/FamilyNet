using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Uploader
{
    public class FileUploader : IFileUploader
    {
        #region private fields

        private readonly IHostingEnvironment _environment;

        #endregion

        #region ctor

        public FileUploader(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        #endregion

        public string CopyFileToServer(string fileName, string directory, IFormFile file)
        {
            var webRoot = _environment.WebRootPath;
            var extension = Path.GetExtension(file.FileName);
            CopyFileFromStream(fileName, directory, file, webRoot, extension);

            return Path.Combine(directory, fileName).Replace("\\", "/") + extension;
        }

        public string CopyFileToClient(string fileName, string directory, IFormFile file)
        {
            var webRoot = _environment.WebRootPath;
            var extension = Path.GetExtension(file.FileName);

            return CopyFileFromStream(fileName, directory, file, webRoot, extension);
        }


        private static string CopyFileFromStream(string fileName, 
                                                 string directory, 
                                                 IFormFile file, 
                                                 string webRoot, 
                                                 string extension)
        {
            var filePath = Path.Combine(webRoot, directory, fileName) + extension;

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            return filePath;
        }
    }
}
