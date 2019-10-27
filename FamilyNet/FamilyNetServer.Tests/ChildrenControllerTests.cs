using DataTransferObjects;
using FamilyNetServer.Configuration;
using FamilyNetServer.Controllers.API;
using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Tests
{
    class ChildrenControllerTests
    {
        #region fields

        private Mock<IUnitOfWorkAsync> _mockUnitOfWork;
        private Mock<IFileUploader> _mockFileUploader;
        private Mock<IChildValidator> _mockChildValidator;
        private Mock<IFilterConditionsChildren> _mockFilterConditions;
        private Mock<IOptionsSnapshot<ServerURLSettings>> _mockSettings;
        private ChildrenController controller;

        #endregion

        #region SetUp

        [SetUp]
        public void Init()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorkAsync>();
            _mockFileUploader = new Mock<IFileUploader>();
            _mockFilterConditions = new Mock<IFilterConditionsChildren>();
            _mockSettings = new Mock<IOptionsSnapshot<ServerURLSettings>>();
            _mockChildValidator = new Mock<IChildValidator>();
            controller = new ChildrenController(_mockFileUploader.Object,
                                                _mockUnitOfWork.Object,
                                                _mockChildValidator.Object,
                                                _mockFilterConditions.Object,
                                                _mockSettings.Object);
        }

        #endregion

        [Test]
        public void ChildrenController_WithFilter_ShouldReturnListOfChildren()
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();

            var orphans = new List<Orphan>()
            {
                new Orphan()
                {
                    IsDeleted = false,
                    FullName = new FullName()
                    {
                        Name = "Name",
                        Patronymic= "Patronymic",
                        Surname = "Surname"
                    }
                }
            }.AsQueryable();

            _mockOrphanRepository.Setup(r => r.GetAll()).Returns(orphans);

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);

            var filter = It.IsAny<FilterParemetersChildren>();
            controller.GetAll(filter);

            _mockOrphanRepository.Verify(m => m.GetAll(), Times.Once);
        }


        [Test]
        [TestCase(1)]
        public async Task ChildrenController_WithValidId_ShouldReturnChildById(int id)
        {
            var mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();

            var orphan = new Orphan()
            {
                IsDeleted = false,
                FullName = new FullName()
                {
                    Name = "Name",
                    Patronymic = "Patronymic",
                    Surname = "Surname"
                }
            };

            mockOrphanRepository.Setup(r => r.GetById(id)).ReturnsAsync(orphan);
            var settings = new ServerURLSettings() { ServerURL = "" };

            _mockSettings.Setup(m => m.Value).Returns(() => settings);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(mockOrphanRepository.Object);

            await controller.Get(id);

            mockOrphanRepository.Verify(m => m.GetById(id), Times.Once);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public async Task ChildrenController_WithInValidId_ShouldReturnChildById(int id)
        {
            var mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            mockOrphanRepository.Setup(m => m.GetById(id)).ReturnsAsync(() => null);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(mockOrphanRepository.Object);

            var result = await controller.Get(id);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }


        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCallMockFileUploader()
        {
            var childDTO = new ChildDTO()
            {
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockFileUploader.Verify(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()),
                                        Times.Once);
        }

        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCallChildValidator()
        {
            var childDTO = new ChildDTO()
            {
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockChildValidator.Verify(m => m.IsValid(childDTO), Times.Once);
        }


        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCallRepositoryCreate()
        {
            var childDTO = new ChildDTO()
            {
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockOrphanRepository.Verify(m => m.Create(It.IsAny<Orphan>()), Times.Once);
        }

        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCallUnitOfWorkSaveChanges()
        {
            var childDTO = new ChildDTO()
            {
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldReturnCreated()
        {
            var childDTO = new ChildDTO()
            {
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Create(childDTO);

            Assert.IsInstanceOf<CreatedResult>(result);
        }

        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_ShouldCallChildValidator()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockChildValidator.Verify(m => m.IsValid(childDTO), Times.Once);
        }

        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_ShouldNotCallFileUploader()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockFileUploader.Verify(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()),
                                        Times.Never);
        }

        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_ShouldCallOrphanRepositoryCreate()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockOrphanRepository.Verify(m => m.Create(It.IsAny<Orphan>()), Times.Once);
        }


        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_ShouldCallUniofWorkSaveChanges()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ChildrenController_WithInValidChildDTOWithoutFile_ShouldNotCallFileUploader()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(false);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockFileUploader.Verify(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()),
                                        Times.Never);
        }

        [Test]
        public async Task ChildrenController_WithInValidChildDTOWithoutFile_ShouldCallChildValidator()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(false);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockChildValidator.Verify(m => m.IsValid(childDTO), Times.Once);
        }


        [Test]
        public async Task ChildrenController_WithInValidChildDTOWithoutFile_ShouldNotCallRepositoryCreate()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(false);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);

            _mockOrphanRepository.Verify(m => m.Create(It.IsAny<Orphan>()), Times.Never);
        }

        [Test]
        public async Task ChildrenController_WithInValidChildDTOWithoutFile_ShouldNotCallUnitOfWork()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(false);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            await controller.Create(childDTO);
            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        }


        [Test]
        public async Task ChildrenController_WithInValidChildDTOWithoutFile_ShouldReturnBadRequest()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Create(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(false);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Create(childDTO);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }
                      

                      
        [Test]
        public async Task ChildrenController_WithInValidChildDTOWithoutFile_ShouldNoatCallRepositoryUpdate()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(false);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockOrphanRepository.Verify(m => m.Update(It.IsAny<Orphan>()), Times.Never);
        }



        [Test]
        public async Task ChildrenController_WithInValidChildDTOWithoutFile_ShouldNotCallUnitOfWorkSaveChanges()
        {
            var childDTO = new ChildDTO();

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(false);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        }


        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCallFileUploader()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockFileUploader.Verify(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()),
                                        Times.Once);
        }


        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCalChildValidator()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);
            _mockChildValidator.Verify(m => m.IsValid(childDTO), Times.Once);
        }


        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCallRepositoryUpdate()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockOrphanRepository.Verify(m => m.Update(It.IsAny<Orphan>()), Times.Once);
        }


        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldCallUnitOfWork()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ChildrenController_WithValidChildDTOWithFile_ShouldReturnNoContent()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
                Avatar = new Mock<IFormFile>().Object
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);
            Assert.IsInstanceOf<NoContentResult>(result);
        }



        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_UpdateShouldNotCallFileUploader()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockFileUploader.Verify(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()),
                                        Times.Never);
        }




        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFileUpdate_ShouldCallChildValidator()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockChildValidator.Verify(m => m.IsValid(childDTO), Times.Once);
        }



        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_ShouldRepositoryUpdateChild()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);
            _mockOrphanRepository.Verify(m => m.Update(It.IsAny<Orphan>()), Times.Once);
        }




        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_ShouldCallUnitofWorkSaveChange()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        }



        [Test]
        public async Task ChildrenController_WithValidChildDTOWithoutFile_ShouldReturnNoContent()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new System.DateTime(2018, 1, 1),
                ChildrenHouseID = 2,
                Surname = "Surname",
                ID = 5,
                Name = "Name",
                Patronymic = "Patronymic",
                ChildrenHouseName = "ChildrenHouseName",
            };

            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(r => r.Update(It.IsNotNull<Orphan>()));
            _mockOrphanRepository.Setup(m => m.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockChildValidator.Setup(m => m.IsValid(childDTO)).Returns(true);
            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());
            _mockFileUploader.Setup(m => m.CopyFileToServer(It.IsAny<string>(),
                                        It.IsAny<string>(), It.IsAny<IFormFile>()))
                                        .Returns(It.IsAny<string>());

            var result = await controller.Edit(It.IsAny<int>(), childDTO);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(300)]
        public async Task ChildrenController_WithValidId_ShouldCallRepositoryUpdate(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);

            _mockOrphanRepository.Verify(m => m.Update(It.IsAny<Orphan>()), Times.Once);
        }

        [Test]
        [TestCase(1)]
        [TestCase(300)]
        public async Task ChildrenController_WithValidId_ShouldCallRepositoryGetById(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);

            _mockOrphanRepository.Verify(m => m.GetById(id), Times.Once);
        }

        [Test]
        [TestCase(1)]
        [TestCase(300)]
        public async Task ChildrenController_WithValidId_ShouldCallUnitOfWorkSaveChanges(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);
            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase(1)]
        [TestCase(300)]
        public async Task ChildrenController_WithValidId_ShouldReturnOkResult(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => new Orphan() { FullName = new FullName() });

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);

            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        public async Task ChildrenController_WithInValidId_ShouldNotCallRepositoryUpdate(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => null);

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);

            _mockOrphanRepository.Verify(m => m.Update(It.IsAny<Orphan>()), Times.Never);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        public async Task ChildrenController_WithInValidId_ShouldNotRepositoryCallGetById(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => null);

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);

            _mockOrphanRepository.Verify(m => m.GetById(id), Times.Never);
        }


        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        public async Task ChildrenController_WithInValidId_ShouldNotCallUnitOfWorkSaveChanges(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => null);

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);
            _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        }


        [Test]
        [TestCase(-10)]
        [TestCase(0)]
        public async Task ChildrenController_WithInValidId_ShouldReturnBadRequest(int id)
        {
            var _mockOrphanRepository = new Mock<IAsyncRepository<Orphan>>();
            _mockOrphanRepository.Setup(m => m.GetById(id))
                .ReturnsAsync(() => null);

            _mockUnitOfWork.Setup(m => m.Orphans).Returns(_mockOrphanRepository.Object);
            _mockUnitOfWork.Setup(m => m.SaveChangesAsync());

            var result = await controller.Delete(id);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }
    }
}
