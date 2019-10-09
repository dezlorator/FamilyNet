using FamilyNetServer.DTO;
using FamilyNetServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChildrenController : ControllerBase
    {
        private readonly IHostingEnvironment _environment;

        public ChildrenController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromForm]ChildDTO orphan)
        {
            var child = new Orphan()
            {
                Rating = orphan.Rating,
                Birthday = orphan.Birthday,
                FullName = orphan.FullName,
                OrphanageID = orphan.OrphanageID,
            };

            var image = orphan.Avatar;

            var webRoot = _environment.WebRootPath;
            var nameFile = child.FullName + child.Birthday.ToShortDateString() + DateTime.Now.Ticks;
            var file = Path.Combine(webRoot, "Children", nameFile);

            child.Avatar = nameFile;

            if (image.Length > 0)
            {
                using (var fileStream = new FileStream(nameFile, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }

            return Ok(new { status = true, message = "Child Posted Successfully" });
        }
    }
}