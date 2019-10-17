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

        public Stream CopyFileToStream(IFormFile file)
        {
            Stream stream = null;

            if (file.Length > 0)
            {
                stream = new MemoryStream();
                file.CopyTo(stream);
                stream.Position = 0;
            }

            return stream;
        }
    }
}
