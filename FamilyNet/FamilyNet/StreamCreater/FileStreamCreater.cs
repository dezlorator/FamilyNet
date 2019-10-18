using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FamilyNet.StreamCreater
{
    public class FileStreamCreater : IFileStreamCreater
    {
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
