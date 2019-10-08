using FamilyNetServer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Infrastructure
{
    public class ImageHelper
    {
        IFormFile _image;

        string Exstention;

        ImageHelper(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                _image = image;
                Exstention = string.Join("", image.FileName.TakeLast(4));
            }
            else throw new ArgumentException(image == null ? "image is null" : "image lenght equal 0"); // TODO : rewrite class to clear from exceptions

        }

        public static async Task SetAvatar(IAvatar obj, IFormFile file, string path)
        {
            try
            {
                ImageHelper imageHelper = new ImageHelper(file);
                obj.Avatar = await imageHelper.GetImageName(path);
            }
            catch (ArgumentException ex)
            {
                //file wasn`t uploaded
            }
        }

        /// <summary>
        /// Save and return image name
        /// </summary>
        /// <param name="path">folder or path in wwwroot</param>
        /// <returns></returns>
        private async Task<string> GetImageName(string path)
        {
            var fileName = Path.GetRandomFileName();
            fileName = Path.ChangeExtension(fileName, Exstention);
            await SaveImage(path, fileName);
            return fileName;
        }

        private async Task SaveImage(string path, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), path, fileName);
            using (var fileSteam = new FileStream(filePath, FileMode.Create))
            {
                await _image.CopyToAsync(fileSteam);
            }
        }
    }
}
