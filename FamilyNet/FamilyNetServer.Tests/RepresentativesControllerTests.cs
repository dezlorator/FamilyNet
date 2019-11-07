using NUnit.Framework;
using Moq;
using FamilyNetServer.Controllers.API;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using FamilyNetServer.Filters;
using FamilyNetServer.Configuration;
using Microsoft.Extensions.Options;
using FamilyNetServer.Models;
using System.Collections.Generic;
using System.Linq;
using FamilyNetServer.Filters.FilterParameters;
using DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using FamilyNetServer.Controllers.API.V1;

namespace FamilyNetServer.Tests
{
    [TestFixture]
    class RepresentativesControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IFileUploader> _mockFileUploader;
        private Mock<IRepresentativeValidator> _mockValidator;
        private Mock<IFilterConditionsRepresentatives> _mockFilterConditions;
        private Mock<IRepository<Representative>> _mockRepresentatives;
        private Mock<IOptionsSnapshot<ServerURLSettings>> _mockSettings;
        private RepresentativesController _controller;

        [SetUp]
        public virtual void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFileUploader = new Mock<IFileUploader>();
            _mockFilterConditions = new Mock<IFilterConditionsRepresentatives>();
            _mockValidator = new Mock<IRepresentativeValidator>();
            _mockRepresentatives = new Mock<IRepository<Representative>>();
            _mockSettings = new Mock<IOptionsSnapshot<ServerURLSettings>>();
            _controller = new RepresentativesController(_mockFileUploader.Object,
                                                _mockUnitOfWork.Object,
                                                _mockValidator.Object,
                                                _mockFilterConditions.Object,
                                                _mockSettings.Object);
            _mockUnitOfWork.Setup(t => t.Representatives).Returns(_mockRepresentatives.Object);
        }

        #region GetAll

        [Test]
        public void GetAll_WithFilter_ShouldRequestListOfRepresentativesFromDB()
        {
            //Arrange
            var representatives = new List<Representative>()
            {
                new Representative()
                    {
                    ID = 1,
                    FullName = new FullName()
                        {
                            Name = "Иван",
                            Surname = "Ивановов",
                            Patronymic = "Иванович"
                        },
                    IsDeleted = false
                    }
            }.AsQueryable();
            _mockRepresentatives.Setup(repo => repo.GetAll()).Returns(representatives);

            //Act
            _controller
                .GetAll(It.IsAny<FilterParametersRepresentatives>());

            //Assert
            _mockRepresentatives.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Test]
        public void GetAll_WithFilter_ShouldReturnOkObjectResult()
        {
            //Arrange
            var representatives = new List<Representative>()
            {
                new Representative()
                    {
                    ID = 1,
                    FullName = new FullName()
                        {
                            Name = "Иван",
                            Surname = "Ивановов",
                            Patronymic = "Иванович"
                        },
                    IsDeleted = false
                    }
            }.AsQueryable();
            _mockRepresentatives.Setup(repo => repo.GetAll())
                .Returns(representatives);

            var filter = It.IsAny<FilterParametersRepresentatives>();

            _mockRepresentatives.Setup(repo => repo.GetAll())
                .Returns(representatives);
            _mockFilterConditions
                .Setup(f => f.GetRepresentatives(representatives, filter))
                .Returns(representatives);

            //Act
            var result = _controller
                .GetAll(filter);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAll_WithFilter_ShouldReturnBadRequestResult()
        {
            //Arrange
            var representatives = new List<Representative>
            {
                new Representative()
                {
                    ID = 1,
                    FullName = new FullName()
                    {
                        Name = "Иван",
                        Surname = "Ивановов",
                        Patronymic = "Иванович"
                    },
                    IsDeleted = false
                }
            }.AsQueryable();
            var filter = It.IsAny<FilterParametersRepresentatives>();
            _mockRepresentatives.Setup(repo => repo.GetAll()).Returns(representatives);
            _mockFilterConditions.Setup(f => f.GetRepresentatives(representatives, filter)).Returns(() => null);

            //Act
            var result = _controller.GetAll(filter);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        #endregion

        #region GetById

        [Test]
        [TestCase(1)]
        public async Task GetById_WithValidID_ShouldRequestRepresentativeFromDB(int id)
        {
            //Arrange
            var representative = new Representative()
            {
                ID = 1,
                FullName = new FullName()
                {
                    Name = "Иван",
                    Surname = "Ивановов",
                    Patronymic = "Иванович"
                },
                IsDeleted = false
            };
            _mockRepresentatives.Setup(repo => repo.GetById(id)).ReturnsAsync(representative);
            var settings = new ServerURLSettings() { ServerURL = "" };
            _mockSettings.Setup(m => m.Value).Returns(settings);

            //Act
            await _controller.Get(id);

            //Assert
            _mockRepresentatives.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Test]
        [TestCase(1)]
        public async Task GetById_WithValidID_ShouldReturnOkObjectResult(int id)
        {
            //Arrange
            var representative = new Representative()
            {
                ID = 1,
                FullName = new FullName()
                {
                    Name = "Иван",
                    Surname = "Ивановов",
                    Patronymic = "Иванович"
                },
                IsDeleted = false
            };
            _mockRepresentatives.Setup(repo => repo.GetById(id)).ReturnsAsync(representative);
            var settings = new ServerURLSettings() { ServerURL = "" };
            _mockSettings.Setup(m => m.Value).Returns(settings);


            //Act
            var result = await _controller.Get(id);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task GetById_WithInvalidID_ShouldReturnBadRequestResult(int id)
        {
            //Arrange

            _mockRepresentatives.Setup(repo => repo.GetById(id)).ReturnsAsync(() => null);
            _mockUnitOfWork.Setup(t => t.Representatives).Returns(_mockRepresentatives.Object);

            //Act
            var result = await _controller.Get(id);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        #endregion

        #region Create

        [Test]
        public async Task Create_WithRepresentativeDTO_ShouldCallRepresentativeValidator()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            var result = await _controller.Create(representativeDTO);

            //Assert
            _mockValidator.Verify(v => v.IsValid(representativeDTO), Times.Once);
        }

        [Test]
        public async Task Create_WithRepresentativeDTO_ShouldReturnBadRequestResult()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            var result = await _controller.Create(representativeDTO);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Create_WithRepresentativeDTO_ShouldCallFileUploader()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                Avatar = new Mock<IFormFile>().Object
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);

            //Act
            await _controller.Create(representativeDTO);

            //Assert
            _mockFileUploader
                .Verify(u => u.CopyFileToServer(It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<IFormFile>()), Times.Once);
        }

        [Test]
        public async Task Create_WithRepresentativeDTO_ShouldCreateRepresentativeInTheDB()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);

            //Act
            await _controller.Create(representativeDTO);

            //Assert
            _mockRepresentatives.Verify(r => r.Create(It.IsAny<Representative>()),
                                                             Times.Once);
        }

        [Test]
        public async Task Create_WithRepresentativeDTO_ShouldCallSaveChangesAsyncOnTheRepo()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);

            //Act
            await _controller.Create(representativeDTO);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Create_WithRepresentativeDTO_ShouldReturnCreatedResult()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);


            //Act
            var result = await _controller.Create(representativeDTO);

            //Assert
            Assert.IsInstanceOf<CreatedResult>(result);
        }

        [Test]
        public async Task Create_WithRepresentativeDTOWithoutAvatar_ShouldNotCallFileUploader()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);

            //Act
            await _controller.Create(representativeDTO);

            //Assert
            _mockFileUploader
                .Verify(u => u.CopyFileToServer(It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Create_WithInvalidRepresentativeDTO_ShouldNotCallFileUploader()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            await _controller.Create(representativeDTO);

            //Assert
            _mockFileUploader
                .Verify(u => u.CopyFileToServer(It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Create_WithInvalidRepresentativeDTO_ShouldNotCreateRepresentativeInTheDB()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            await _controller.Create(representativeDTO);

            //Assert
            _mockRepresentatives.Verify(r => r.Create(It.IsAny<Representative>()),
                                                             Times.Never);
        }

        [Test]
        public async Task Create_WithInvalidRepresentativeDTO_ShouldNotCallSaveChangesAsyncOnTheRepo()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            await _controller.Create(representativeDTO);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region Edit

        [Test]
        public async Task Edit_WithRepresentativeDTO_ShouldCallGetById()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockRepresentatives.Verify(v => v.GetById(It.IsAny<int>()), Times.Once);
        }
         [Test]
        public async Task Edit_WithRepresentativeDTO_ShouldNotCallGetById()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockRepresentatives.Verify(v => v.GetById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task Edit_WithRepresentativeDTO_ShouldCallRepresentativeValidator()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockValidator.Verify(v => v.IsValid(representativeDTO), Times.Once);
        }

        [Test]
        public async Task Edit_WithInvalidRepresentativeDTO_ShouldReturnBadRequestResult()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            var result = await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Edit_WithRepresentativeDTO_ShouldCallFileUploader()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                Name = "Ivan",
                Avatar = new Mock<IFormFile>().Object
            };

            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro" },
                Avatar = "filePath"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);
            _mockRepresentatives.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(representative);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockFileUploader
                .Verify(u => u.CopyFileToServer(It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<IFormFile>()), Times.Once);
        }

        [Test]
        public async Task Edit_WithRepresentativeDTO_ShouldUpdateRepresentativeInTheDB()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                Name = "Ivan",
                Avatar = new Mock<IFormFile>().Object
            };

            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro" },
                Avatar = "filePath"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);
            _mockRepresentatives.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(representative);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockRepresentatives.Verify(r => r.Update(It.IsAny<Representative>()),
                                                             Times.Once);
        }

        [Test]
        public async Task Edit_WithRepresentativeDTO_ShouldCallSaveChangesAsyncOnTheRepo()
        {
            //Arrange
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                Name = "Ivan",
                Avatar = new Mock<IFormFile>().Object
            };

            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro" },
                Avatar = "filePath"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);
            _mockRepresentatives.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(representative);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Edit_WithRepresentativeDTO_ShouldReturnNoContentResult()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                Name = "Ivan",
                Avatar = new Mock<IFormFile>().Object
            };

            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro" },
                Avatar = "filePath"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);
            _mockRepresentatives.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(representative);

            //Act
            var result = await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
        
        [Test]
        public async Task Edit_WithRepresentativeDTOWithoutFile_ShouldReturnNoContentResult()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                Birthday = new System.DateTime(1990, 1, 6),
                ChildrenHouseID = 3,
                Surname = "Ivanov",
                ID = 2,
                Name = "Ivan",
                Patronymic = "Ivanovich",
                EmailID = 4,
                Rating = 5.0F
            };

            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro" },
                Avatar = "filePath"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);
            _mockRepresentatives.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(representative);

            //Act
            var result = await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
        
        [Test]
        public async Task Edit_WithRepresentativeDTOWithInvalidID_ShouldReturnBadRequestResult()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                Birthday = new System.DateTime(1990, 1, 6),
                ChildrenHouseID = 3,
                Surname = "Ivanov",
                ID = 2,
                Name = "Ivan",
                Patronymic = "Ivanovich",
                EmailID = 4,
                Rating = 5.0F
            };

            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro" },
                Avatar = "filePath"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);
            _mockRepresentatives.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var result = await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Edit_WithRepresentativeDTOWithoutAvatar_ShouldNotCallFileUploader()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(true);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockFileUploader
                .Verify(u => u.CopyFileToServer(It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Edit_WithInvalidRepresentativeDTO_ShouldNotCallFileUploader()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan",
                Avatar = new Mock<IFormFile>().Object
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockFileUploader
                .Verify(u => u.CopyFileToServer(It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Edit_WithInvalidRepresentativeDTO_ShouldNotCreateRepresentativeInTheDB()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockRepresentatives.Verify(r => r.Create(It.IsAny<Representative>()),
                                                             Times.Never);
        }

        [Test]
        public async Task Edit_WithInvalidRepresentativeDTO_ShouldNotCallSaveChangesAsyncOnTheRepo()
        {
            //Arrange
            var representativeDTO = new RepresentativeDTO
            {
                ID = 1,
                Name = "Ivan"
            };
            _mockValidator.Setup(v => v.IsValid(representativeDTO)).Returns(false);

            //Act
            await _controller.Edit(It.IsAny<int>(), representativeDTO);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region Delete

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Delete_WithInvalidID_ShouldReturnBadRequestResult(int id)
        {
            //Act
            var result = await _controller.Delete(id);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        [TestCase(1)]
        public async Task Delete_WithID_ShouldCallRepresentativesGetById(int id)
        {
            //Act
            await _controller.Delete(id);

            //Assert
            _mockRepresentatives.Verify(r => r.GetById(id), Times.Once);
        }

        [Test]
        [TestCase(1)]
        public async Task Delete_WithID_ShouldReturnOkResult(int id)
        {
            //Arrange
            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro", Surname = "Petrov" },
                IsDeleted = false
            };
            _mockRepresentatives.Setup(r => r.GetById(id))
                .ReturnsAsync(representative);

            //Act
            var result = await _controller.Delete(id);

            //Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        [TestCase(1)]
        public async Task Delete_WithID_ShouldCallRepresentativeUpdate(int id)
        {
            //Arrange
            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro", Surname = "Petrov" },
                IsDeleted = false
            };
            _mockRepresentatives.Setup(r => r.GetById(id))
                .ReturnsAsync(representative);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockRepresentatives.Verify(r => r.Update(It.IsAny<Representative>()),
                                                             Times.Once);
        }
        

        [Test]
        [TestCase(1)]
        public async Task Delete_WithID_ShouldCallUnitOfWorkSaveChanges(int id)
        {
            //Arrange
            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro", Surname = "Petrov" },
                IsDeleted = true
            };
            _mockRepresentatives.Setup(r => r.GetById(id))
                .ReturnsAsync(representative);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Delete_WithInvalidID_ShouldNotCallRepresentativesGetById(int id)
        {
            //Act
            await _controller.Delete(id);

            //Assert
            _mockRepresentatives.Verify(r => r.GetById(id), Times.Never);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Delete_WithID_ShouldNotCallRepresentativeUpdate(int id)
        {
            //Arrange
            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro", Surname = "Petrov" },
                IsDeleted = false
            };
            _mockRepresentatives.Setup(r => r.GetById(id))
                .ReturnsAsync(representative);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockRepresentatives.Verify(r => r.Update(It.IsAny<Representative>()),
                                                             Times.Never);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Delete_WithInvalidID_ShouldNotCallUnitOfWorkSaveChanges(int id)
        {
            //Arrange
            var representative = new Representative
            {
                FullName = new FullName { Name = "Petro", Surname = "Petrov" },
                IsDeleted = true
            };
            _mockRepresentatives.Setup(r => r.GetById(id))
                .ReturnsAsync(representative);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [TestCase(1)]
        public async Task Delete_WithValidID_ShouldNotReturnBadRequestResultIfRepresentativeNotExist(int id)
        {
            //Arrange
            _mockRepresentatives.Setup(r => r.GetById(id))
                .ReturnsAsync(() => null);

            //Act
            var result = await _controller.Delete(id);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        #endregion
    }
}
