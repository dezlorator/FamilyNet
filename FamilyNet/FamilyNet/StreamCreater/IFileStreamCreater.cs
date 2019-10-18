using System.IO;
using Microsoft.AspNetCore.Http;

namespace FamilyNet.StreamCreater
{
    public interface IFileStreamCreater
    {
        Stream CopyFileToStream(IFormFile file);
    }
}
