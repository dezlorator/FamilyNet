using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;

namespace FamilyNetServer.Uploaders
{
    public class FileUploader : IFileUploader
    {
        #region private fields

        private readonly IHostingEnvironment _environment;
        private readonly ILogger<FileUploader> _logger;

        #endregion

        #region ctor

        public FileUploader(IHostingEnvironment environment, 
                            ILogger<FileUploader> logger)
        {
            _environment = environment;
            _logger = logger;
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
                    _logger.LogInformation("File was copied into stream.");
                }
            }

            return Path.Combine(directory, fileName).Replace("\\", "/") + extension;
        }
    }
}
