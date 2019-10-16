using Microsoft.AspNetCore.Http;
using System.IO;

namespace FamilyNet.StreamCreater
{
    public interface IFileStreamCreater
    {
        Stream CopyFileToStream(IFormFile file);
    }
}
