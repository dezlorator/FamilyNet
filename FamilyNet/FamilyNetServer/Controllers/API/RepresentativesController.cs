using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DataTransferObjects;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using FamilyNetServer.Uploaders;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RepresentativesController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IFileUploader _fileUploader;
        private readonly IRepresentativeValidator _representativeValidator;
        private readonly IFilterConditionsRepresentatives _filterConditions;
        private string _url 
        {
            get 
            { 
                return string.Format($"{Request.Scheme}://{Request.Host.Value}/"); 
            } 
        }
        
            #endregion

        #region ctor

        public RepresentativesController(IFileUploader fileUploader,
                                  IUnitOfWorkAsync unitOfWork,
                                  IRepresentativeValidator representativeValidator,
                                  IFilterConditionsRepresentatives filterConditions)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _representativeValidator = representativeValidator;
            _filterConditions = filterConditions;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery] FilterParametersPerson filter)
        {
            var representatives = _unitOfWork.Representatives.GetAll().Where(r => !r.IsDeleted);
            representatives = _filterConditions.GetRepresentatives(representatives, filter);

            if (representatives == null)
            {
                return BadRequest();
            }
            var representativesDTO = representatives.Select(r =>
            new RepresentativeDTO()
            {
                PhotoPath = _url + r.Avatar,
                Birthday = r.Birthday,
                EmailID = r.EmailID,
                ID = r.ID,
                Name = r.FullName.Name,
                Patronymic = r.FullName.Patronymic,
                Surname = r.FullName.Surname,
                ChildrenHouseID = r.OrphanageID,
                Rating = r.Rating
            });

            return Ok(representativesDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var represenntative = await _unitOfWork.Representatives.GetById(id);

            if (represenntative == null)
            {
                return BadRequest();
            }

            var representativeDTO = new RepresentativeDTO()
            {
                Birthday = represenntative.Birthday,
                ID = represenntative.ID,
                Name = represenntative.FullName.Name,
                ChildrenHouseID = represenntative.OrphanageID,
                Patronymic = represenntative.FullName.Patronymic,
                Rating = represenntative.Rating,
                Surname = represenntative.FullName.Surname,
                EmailID = represenntative.EmailID,
                PhotoPath = _url + represenntative.Avatar
            };

            return Ok(representativeDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]RepresentativeDTO representativeDTO)
        {
            if (!_representativeValidator.IsValid(representativeDTO))
            {
                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (representativeDTO.Avatar != null)
            {
                var fileName = representativeDTO.Name + representativeDTO.Surname
                        + representativeDTO.Patronymic + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Representatives), representativeDTO.Avatar);
            }

            var representative = new Representative()
            {
                Rating = representativeDTO.Rating,
                Birthday = representativeDTO.Birthday,
                FullName = new FullName()
                {
                    Name = representativeDTO.Name,
                    Surname = representativeDTO.Surname,
                    Patronymic = representativeDTO.Patronymic
                },

                OrphanageID = representativeDTO.ChildrenHouseID,
                Avatar = pathPhoto,
                EmailID = representativeDTO.EmailID,
            };

            await _unitOfWork.Representatives.Create(representative);
            _unitOfWork.SaveChangesAsync();

            representativeDTO.ID = representative.ID;
            representativeDTO.PhotoPath = representative.Avatar;

            return Created("api/v1/{controller}/" + representative.ID, representativeDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]RepresentativeDTO representativeDTO)
        {
            if (!_representativeValidator.IsValid(representativeDTO))
            {
                return BadRequest();
            }

            var representative = await _unitOfWork.Representatives.GetById(representativeDTO.ID);

            if (representative == null)
            {
                return BadRequest();
            }

            representative.FullName.Name = representativeDTO.Name;
            representative.FullName.Patronymic = representativeDTO.Patronymic;
            representative.FullName.Surname = representativeDTO.Surname;
            representative.Birthday = representativeDTO.Birthday;
            representative.Rating = representativeDTO.Rating;
            representative.OrphanageID = representativeDTO.ChildrenHouseID;
            representative.EmailID = representativeDTO.EmailID;

            if (representativeDTO.Avatar != null)
            {
                var fileName = representativeDTO.Name + representativeDTO.Surname
                        + representativeDTO.Patronymic + DateTime.Now.Ticks;

                representative.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Representatives), representativeDTO.Avatar);
            }

            _unitOfWork.Representatives.Update(representative);
            _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var representative = await _unitOfWork.Representatives.GetById(id);

            if (representative == null)
            {
                return BadRequest();
            }

            representative.IsDeleted = true;

            _unitOfWork.Representatives.Update(representative);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}