using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Controllers.API;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using FamilyNetServer.Models;
using FamilyNetServer.HttpHandlers;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Helpers;
using DataTransferObjects;
using DataTransferObjects.Enums;
using NUnit.Framework;
using Moq;

namespace FamilyNetServer.Tests
{
    [TestFixture]
    class ScheduleControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IAvailabilityValidator> _mockValidator;
        private Mock<IScheduleHelper> _mockHelper;
        private Mock<IRepository<Availability>> _mockAvailabilities;
        private ScheduleController _controller;
        private Mock<IIdentityExtractor> _mockIdentityExtractor;
        private Mock<ILogger<ScheduleController>> _mockLogger;
        private Mock<ClaimsPrincipal> _mockClaimsPrincipal;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IUserStore<ApplicationUser>> _mockUserStore;
        private ApplicationUser _appUser;

        [SetUp]
        public virtual void SetUp()
        {
            _mockLogger = new Mock<ILogger<ScheduleController>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IAvailabilityValidator>();
            _mockHelper = new Mock<IScheduleHelper>();
            _mockAvailabilities = new Mock<IRepository<Availability>>();
            _mockIdentityExtractor = new Mock<IIdentityExtractor>();
            _controller = new ScheduleController(_mockUnitOfWork.Object,
                                                _mockValidator.Object,
                                                _mockLogger.Object,
                                                _mockIdentityExtractor.Object,
                                                _mockHelper.Object);
            _mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            _mockUserStore.Object, null, null, null, null, null, null, null, null);
            _mockUnitOfWork.Setup(t => t.Availabilities).Returns(_mockAvailabilities.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Ivan")
           }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _appUser = new ApplicationUser() { PersonID = 1 };

            _mockUnitOfWork.Setup(t => t.Availabilities).Returns(_mockAvailabilities.Object);
            _mockUnitOfWork.Setup(t => t.UserManager).Returns(_mockUserManager.Object);
            _mockUnitOfWork.Setup(t => t.UserManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(_appUser);
            _mockIdentityExtractor.Setup(e => e.GetId(_mockClaimsPrincipal.Object)).Returns("");
            _mockIdentityExtractor.Setup(e => e.GetSignature(It.IsAny<HttpContext>())).Returns("");
        }

        #region GetAll

        [Test]
        public void GetAll_WithValidAvailabilityInDB_ShouldRequestListOfAvailabilitiesFromDB()
        {
            //Arrange
            var availabilities = new List<Availability>()
            {
                new Availability()
                    {
                    ID = 1,
                    Date = DateTime.Now.AddDays(1),
                    FreeHours = new TimeSpan(1, 0, 0),
                    IsReserved = false,
                    PersonID = 1,
                    QuestID = 0,
                    QuestName = null,
                    Role = PersonType.Volunteer,
                    IsDeleted = false
                    }
            }.AsQueryable();

            _mockAvailabilities.Setup(repo => repo.GetAll()).Returns(availabilities);

            //Act
            var result = _controller.GetAll();

            //Assert
            _mockAvailabilities.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Test]
        public void GetAll_WithValidAvailabilityInDB_ShouldReturnOkObjectResult()
        {
            //Arrange
            var availabilities = new List<Availability>()
            {
                new Availability()
                    {
                    ID = 1,
                    Date = DateTime.Now.AddDays(1),
                    FreeHours = new TimeSpan(1, 0, 0),
                    IsReserved = false,
                    PersonID = 1,
                    QuestID = 0,
                    QuestName = null,
                    Role = PersonType.Volunteer,
                    IsDeleted = false
                    }
            }.AsQueryable();

            _mockAvailabilities.Setup(repo => repo.GetAll()).Returns(availabilities);


            //Act
            var result = _controller.GetAll();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAll_WithInvalidDateInDB_ShouldReturnOkObjectResult()
        {
            //Arrange
            var availabilities = new List<Availability>()
            {
                new Availability()
                    {
                    ID = 1,
                    Date = DateTime.Now.AddDays(-1),
                    FreeHours = new TimeSpan(1, 0, 0),
                    IsReserved = false,
                    PersonID = 1,
                    QuestID = 0,
                    QuestName = null,
                    Role = PersonType.Volunteer,
                    IsDeleted = false
                    }
            }.AsQueryable();

            _mockAvailabilities.Setup(repo => repo.GetAll()).Returns(availabilities);


            //Act
            var result = _controller.GetAll();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAll_WithIsDeletedTrueInDB_ShouldReturnOkObjectResult()
        {
            //Arrange
            var availabilities = new List<Availability>()
            {
                new Availability()
                    {
                    ID = 1,
                    Date = DateTime.Now.AddDays(1),
                    PersonID = 1,
                    Role = PersonType.Volunteer,
                    IsDeleted = true
                    }
            }.AsQueryable();

            _mockAvailabilities.Setup(repo => repo.GetAll()).Returns(availabilities);

            //Act
            var result = _controller.GetAll();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAll_WithAnotherPersonIdInDB_ShouldReturnOkObjectResult()
        {
            //Arrange
            var availabilities = new List<Availability>()
            {
                new Availability()
                    {
                    ID = 1,
                    Date = DateTime.Now.AddDays(1),
                    PersonID = 2,
                    Role = PersonType.Volunteer,
                    IsDeleted = false
                    }
            }.AsQueryable();

            _mockAvailabilities.Setup(repo => repo.GetAll()).Returns(availabilities);

            //Act
            var result = _controller.GetAll();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAll_WithPastDateInDB_ShouldReturnOkObjectResult()
        {
            //Arrange
            var availabilities = new List<Availability>()
            {
                new Availability()
                    {
                    ID = 1,
                    Date = DateTime.Now.AddDays(-1),
                    PersonID = 1,
                    Role = PersonType.Volunteer,
                    IsDeleted = false
                    }
            }.AsQueryable();

            _mockAvailabilities.Setup(repo => repo.GetAll()).Returns(availabilities);

            //Act
            var result = _controller.GetAll();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAll_WithEmptyListFromDB_ShouldReturnOkObjectResult()
        {
            //Arrange
            var empty = new List<Availability>().AsQueryable();
            _mockAvailabilities.Setup(repo => repo.GetAll()).Returns(empty);

            //Act
            var result = _controller.GetAll();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        #region GetFreePersonIDsList

        [Test]
        public void GetFreePersonIDsList_WithValidAvailability_ShouldReturnAvailabilityFromDB()
        {
            //Arrange
            var date = DateTime.Now;
            var duration = new TimeSpan(1, 0, 0);
            var availabilities = new List<Availability>
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddHours(-1),
                    PersonID = 1,
                    FreeHours = new TimeSpan(3, 0, 0),
                    Role = PersonType.Volunteer,
                    IsDeleted = false,
                    IsReserved = false
                }
            }.AsQueryable();

            _mockAvailabilities
                .Setup(repo => repo.Get(It.IsAny<Func<Availability, bool>>()))
                .Returns(availabilities);

            //Act
            var result = _controller.GetFreePersonIDsList(date, duration, PersonType.Volunteer);

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetFreePersonIDsList_WithValidAvailability_ShouldReturnOkObjectResult()
        {
            //Arrange
            var date = DateTime.Now;
            var duration = new TimeSpan(1, 0, 0);
            var availabilities = new List<Availability>
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddHours(-1),
                    PersonID = 1,
                    FreeHours = new TimeSpan(3, 0, 0),
                    Role = PersonType.Volunteer,
                    IsDeleted = false,
                    IsReserved = false
                }
            }.AsQueryable();

            _mockAvailabilities
                .Setup(repo => repo.Get(It.IsAny<Func<Availability, bool>>()))
                .Returns(availabilities);

            //Act
            var result = _controller.GetFreePersonIDsList(date, duration, PersonType.Volunteer);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetFreePersonIDsList_WithInvalidDate_ShouldReturnNotFound()
        {
            //Arrange
            var date = DateTime.Now.AddHours(3);
            var duration = new TimeSpan(1, 0, 0);
            var availabilities = new List<Availability>
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddHours(-1),
                    PersonID = 1,
                    FreeHours = new TimeSpan(1, 0, 0),
                    Role = PersonType.Volunteer,
                    IsDeleted = false,
                    IsReserved = false
                }
            }.AsQueryable();

            _mockAvailabilities
                .Setup(repo => repo.Get(null))
                .Returns(availabilities);

            //Act
            var result = _controller.GetFreePersonIDsList(date, duration, PersonType.Volunteer);

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void GetFreePersonIDsList_WithBigDuration_ShouldReturnNotFoundResult()
        {
            //Arrange
            var date = DateTime.Now;
            var duration = new TimeSpan(4, 0, 0);
            var availabilities = new List<Availability>
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddHours(-1),
                    PersonID = 1,
                    FreeHours = new TimeSpan(2, 0, 0),
                    Role = PersonType.Volunteer,
                    IsDeleted = false,
                    IsReserved = false
                }
            }.AsQueryable();

            _mockAvailabilities
                .Setup(repo => repo.Get(null))
                .Returns(availabilities);

            //Act
            var result = _controller.GetFreePersonIDsList(date, duration, PersonType.Volunteer);

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void GetFreePersonIDsList_WithInvalidRole_ShouldReturnNotFoundResult()
        {
            //Arrange
            var date = DateTime.Now;
            var duration = new TimeSpan(3, 0, 0);
            var availabilities = new List<Availability>
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddHours(-1),
                    PersonID = 1,
                    FreeHours = new TimeSpan(2, 0, 0),
                    Role = PersonType.Volunteer,
                    IsDeleted = false,
                    IsReserved = false
                }
            }.AsQueryable();

            _mockAvailabilities
                .Setup(repo => repo.Get(null))
                .Returns(availabilities);

            //Act
            var result = _controller.GetFreePersonIDsList(date, duration, PersonType.CharityMaker);

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void GetFreePersonIDsList_WithIsDeletedTrue_ShouldReturnNotFoundResult()
        {
            //Arrange
            var date = DateTime.Now;
            var duration = new TimeSpan(3, 0, 0);
            var availabilities = new List<Availability>
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddHours(-1),
                    PersonID = 1,
                    FreeHours = new TimeSpan(2, 0, 0),
                    Role = PersonType.Volunteer,
                    IsDeleted = true,
                    IsReserved = false
                }
            }.AsQueryable();

            _mockAvailabilities
                .Setup(repo => repo.Get(null))
                .Returns(availabilities);

            //Act
            var result = _controller.GetFreePersonIDsList(date, duration, PersonType.CharityMaker);

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void GetFreePersonIDsList_WithIsReservedTrue_ShouldReturnNotFoundResult()
        {
            //Arrange
            var date = DateTime.Now;
            var duration = new TimeSpan(3, 0, 0);
            var availabilities = new List<Availability>
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddHours(-1),
                    PersonID = 1,
                    FreeHours = new TimeSpan(2, 0, 0),
                    Role = PersonType.Volunteer,
                    IsDeleted = false,
                    IsReserved = true
                }
            }.AsQueryable();

            _mockAvailabilities
                .Setup(repo => repo.Get(null))
                .Returns(availabilities);

            //Act
            var result = _controller.GetFreePersonIDsList(date, duration, PersonType.CharityMaker);

            //Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        #endregion

        #region GetByID

        [Test]
        [TestCase(1)]
        public async Task GetById_WithValidID_ShouldRequestAvailabilityFromDB(int id)
        {
            //Arrange
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(-1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(availabilitiy);

            //Act
            var result = await _controller.GetById(id);

            //Assert
            _mockAvailabilities.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Test]
        [TestCase(1)]
        public async Task GetById_WithValidID_ShouldReturnOkObjectResult(int id)
        {
            //Arrange
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(-1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(availabilitiy);

            //Act
            var result = await _controller.GetById(id);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        [TestCase(-1)]
        public async Task GetById_WithInValidID_ShouldReturnBadRequestResult(int id)
        {
            //Arrange
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(() => null);

            //Act
            var result = await _controller.GetById(id);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        #endregion

        #region SwitchReserveStatusById

        [Test]
        [TestCase(1)]
        public async Task SwitchReserveStatusById_WithValidID_ShouldRequestAvailabilityFromDB(int id)
        {
            //Arrange
            var questName = "name";
            var questId = 1;
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(-1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false,
                IsReserved = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(availabilitiy);

            //Act
            var result = await _controller.SwitchReserveStatusById(id, questName, questId);

            //Assert
            _mockAvailabilities.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Test]
        [TestCase(1)]
        public async Task SwitchReserveStatusById_WithValidID_ShouldRepoCallUpdateMethod(int id)
        {
            //Arrange
            var questName = "name";
            var questId = 1;
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(-1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false,
                IsReserved = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(availabilitiy);

            //Act
            var result = await _controller.SwitchReserveStatusById(id, questName, questId);

            //Assert
            _mockAvailabilities.Verify(repo => repo.Update(It.IsAny<Availability>()), Times.Once);
        }
        
        [Test]
        [TestCase(1)]
        public async Task SwitchReserveStatusById_WithValidID_ShouldUnitOfWorkSaveChanges(int id)
        {
            //Arrange
            var questName = "name";
            var questId = 1;
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(-1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false,
                IsReserved = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(availabilitiy);

            //Act
            var result = await _controller.SwitchReserveStatusById(id, questName, questId);

            //Assert
            _mockUnitOfWork.Verify(repo => repo.SaveChanges(), Times.Once);
        }

        [Test]
        [TestCase(1)]
        public async Task SwitchReserveStatusById_WithValidID_ShouldReturnOkObjectResult(int id)
        {
            //Arrange
            var questName = "name";
            var questId = 1;
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(-1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false,
                IsReserved = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(availabilitiy);

            //Act
            var result = await _controller.SwitchReserveStatusById(id, questName, questId);

            //Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        [TestCase(-1)]
        public async Task SwitchReserveStatusById_WithInValidID_ShouldReturnBadRequestResult(int id)
        {
            //Arrange
            var questName = "name";
            var questId = 1;
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false,
                IsReserved = false
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(() => null);

            //Act
            var result = await _controller.SwitchReserveStatusById(id, questName, questId);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }
        
        [Test]
        [TestCase(-1)]
        public async Task SwitchReserveStatusById_WithIsReservedTrue_ShouldReturnBadRequestResult(int id)
        {
            //Arrange
            var questName = "name";
            var questId = 1;
            var availabilitiy = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer,
                IsDeleted = false,
                IsReserved = true
            };
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(() => null);

            //Act
            var result = await _controller.SwitchReserveStatusById(id, questName, questId);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        #endregion

        #region Create

        [Test]
        public async Task Create_WithAvailabilityDTO_ShouldCallAvailabilityValidator()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(true);

            //Act
            var result = await _controller.Create(availabilityDTO);

            //Assert
            _mockValidator.Verify(v => v.IsValid(availabilityDTO), Times.Once);
        }

        [Test]
        public async Task Create_WithInvalidAvailabilityDTO_ShouldReturnBadRequestResult()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(false);

            //Act
            var result = await _controller.Create(availabilityDTO);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Create_WithAvailabilityDTO_ShouldCreateAvailabilityInTheDB()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(true);

            //Act
            var result = await _controller.Create(availabilityDTO);

            //Assert
            _mockAvailabilities.Verify(a => a.Create(It.IsAny<Availability>()), Times.Once);
        }

        [Test]
        public async Task Create_WithAvailabilityDTO_ShouldCallUnitOfWorkSaveChangesAsync()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(true);

            //Act
            var result = await _controller.Create(availabilityDTO);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task Create_WithAvailabilityDTO_ShouldReturnCreatedResult()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(true);

            //Act
            var result = await _controller.Create(availabilityDTO);

            //Assert
            Assert.IsInstanceOf<CreatedResult>(result);
        }

        [Test]
        public async Task Create_WithAvailabilityDTO_ShouldNotCallSaveChangesAsyncOnTheRepo()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(false);

            //Act
            var result = await _controller.Create(availabilityDTO);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChanges(), Times.Never);
        }

        [Test]
        public async Task Create_WhithOverlapingDate_ShouldReturnConflictResult()
        {
            //Arrange
            var availabilities = new List<Availability>()
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddDays(2),
                    FreeHours = new TimeSpan(3, 0, 0),
                    IsReserved = false,
                    PersonID = 1,
                    QuestID = 0,
                    QuestName = null,
                    Role = PersonType.Volunteer,
                    IsDeleted = false
                }
            }.AsQueryable();

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                DayOfWeek = DateTime.Now.AddDays(2).DayOfWeek,
                StartTime = DateTime.Now,
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockAvailabilities.Setup(repo => repo.Get(It.IsAny<Func<Availability, bool>>())).Returns(availabilities);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockValidator.Setup(v => v.IsValid(It.IsAny<AvailabilityDTO>())).Returns(true);
            _mockValidator.Setup(v => v.IsOverlaping(It.IsAny<AvailabilityDTO>(),
                It.IsAny<Availability>())).Returns(true);

            //Act
            var result = await _controller.Create(availabilityDTO);

            //Assert
            Assert.IsInstanceOf<ConflictResult>(result);
        }

        #endregion

        #region Edit

        [Test]
        public async Task Edit_WhithAvailabilityDTO_ShouldCallUpdateMethod()
        {
            //Arrange
            var id = 1;
            var availability = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
                IsReserved = false,
                PersonID = 1,
                QuestID = 0,
                QuestName = null,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                DayOfWeek = DateTime.Now.AddDays(2).DayOfWeek,
                StartTime = DateTime.Now,
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockAvailabilities.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(availability);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockValidator.Setup(v => v.IsValid(It.IsAny<AvailabilityDTO>())).Returns(true);
            _mockValidator.Setup(v => v.IsOverlaping(It.IsAny<AvailabilityDTO>(),
                It.IsAny<Availability>())).Returns(false);

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            _mockAvailabilities.Verify(a => a.Update(It.IsAny<Availability>()), Times.Once);
        }

        [Test]
        public async Task Edit_WhithAvailabilityDTO_ShouldCallUnitOfWorkSaveChanges()
        {
            //Arrange
            var id = 1;
            var availability = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
                IsReserved = false,
                PersonID = 1,
                QuestID = 0,
                QuestName = null,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                DayOfWeek = DateTime.Now.AddDays(2).DayOfWeek,
                StartTime = DateTime.Now,
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockAvailabilities.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(availability);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockValidator.Setup(v => v.IsValid(It.IsAny<AvailabilityDTO>())).Returns(true);
            _mockValidator.Setup(v => v.IsOverlaping(It.IsAny<AvailabilityDTO>(),
                It.IsAny<Availability>())).Returns(false);

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task Edit_WhithAvailabilityDTO_ShouldNotCallUpdateMethod()
        {
            //Arrange
            var id = 1;
            var availability = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
                IsReserved = false,
                PersonID = 1,
                QuestID = 0,
                QuestName = null,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                DayOfWeek = DateTime.Now.AddDays(2).DayOfWeek,
                StartTime = DateTime.Now,
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockAvailabilities.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(availability);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockValidator.Setup(v => v.IsValid(It.IsAny<AvailabilityDTO>())).Returns(false);
            _mockValidator.Setup(v => v.IsOverlaping(It.IsAny<AvailabilityDTO>(),
                It.IsAny<Availability>())).Returns(false);

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            _mockAvailabilities.Verify(a => a.Update(It.IsAny<Availability>()), Times.Never);
        }

        [Test]
        public async Task Edit_WhithAvailabilityDTO_ShouldNotCallUnitOfWorkSaveChanges()
        {
            //Arrange
            var id = 1;
            var availability = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
                IsReserved = false,
                PersonID = 1,
                QuestID = 0,
                QuestName = null,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                DayOfWeek = DateTime.Now.AddDays(2).DayOfWeek,
                StartTime = DateTime.Now,
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockAvailabilities.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(availability);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockValidator.Setup(v => v.IsValid(It.IsAny<AvailabilityDTO>())).Returns(false);
            _mockValidator.Setup(v => v.IsOverlaping(It.IsAny<AvailabilityDTO>(),
                It.IsAny<Availability>())).Returns(false);

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never);
        }

        [Test]
        public async Task Edit_WhithAvailabilityDTO_ShouldReturnNoContentResult()
        {
            //Arrange
            var id = 1;
            var availability = new Availability()
            {
                ID = 1,
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
                IsReserved = false,
                PersonID = 1,
                QuestID = 0,
                QuestName = null,
                Role = PersonType.Volunteer,
                IsDeleted = false
            };

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                DayOfWeek = DateTime.Now.AddDays(2).DayOfWeek,
                StartTime = DateTime.Now,
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockAvailabilities.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(availability);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockValidator.Setup(v => v.IsValid(It.IsAny<AvailabilityDTO>())).Returns(true);
            _mockValidator.Setup(v => v.IsOverlaping(It.IsAny<AvailabilityDTO>(),
                It.IsAny<Availability>())).Returns(false);

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        [TestCase(1)]
        public async Task Edit_WithAvailabilityDTO_ShouldCallAvailabilityValidator(int id)
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(true);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            _mockValidator.Verify(v => v.IsValid(availabilityDTO), Times.Once);
        }

        [Test]
        [TestCase(1)]
        public async Task Edit_WithInvalidAvailabilityDTO_ShouldReturnBadRequestResult(int id)
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(false);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        [TestCase(-1)]
        public async Task Edit_WithInvalidId_ShouldReturnBadRequestResult(int id)
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                StartTime = DateTime.Now.AddDays(1),
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockValidator.Setup(v => v.IsValid(availabilityDTO)).Returns(true);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockAvailabilities.Setup(repo => repo.GetById(id)).ReturnsAsync(() => null);

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Edit_WhithOverlapingDate_ShouldReturnConflictResult()
        {
            //Arrange
            var id = 1;
            var availabilities = new List<Availability>()
            {
                new Availability()
                {
                    ID = 1,
                    Date = DateTime.Now.AddDays(2),
                    FreeHours = new TimeSpan(3, 0, 0),
                    IsReserved = false,
                    PersonID = 1,
                    QuestID = 0,
                    QuestName = null,
                    Role = PersonType.Volunteer,
                    IsDeleted = false
                }
            }.AsQueryable();

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = 1,
                DayOfWeek = DateTime.Now.AddDays(2).DayOfWeek,
                StartTime = DateTime.Now,
                PersonID = 1,
                Role = PersonType.Volunteer
            };
            _mockAvailabilities.Setup(repo => repo.Get(It.IsAny<Func<Availability, bool>>())).Returns(availabilities);
            _mockHelper.Setup(h => h.AdjustDate(It.IsAny<AvailabilityDTO>())).Returns(It.IsAny<int>());
            _mockValidator.Setup(v => v.IsValid(It.IsAny<AvailabilityDTO>())).Returns(true);
            _mockValidator.Setup(v => v.IsOverlaping(It.IsAny<AvailabilityDTO>(),
                It.IsAny<Availability>())).Returns(true);

            //Act
            var result = await _controller.Edit(id, availabilityDTO);

            //Assert
            Assert.IsInstanceOf<ConflictResult>(result);
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
        public async Task Delete_WithID_ShouldCallAvailabilitiesGetById(int id)
        {
            //Act
            await _controller.Delete(id);

            //Assert
            _mockAvailabilities.Verify(r => r.GetById(id), Times.Once);
        }

        [Test]
        [TestCase(1)]
        public async Task Delete_WithID_ShouldReturnOkResult(int id)
        {
            //Arrange
            var availability = new Availability()
            {
                ID = 1,
                IsDeleted = false
            };

            _mockAvailabilities.Setup(r => r.GetById(id))
                .ReturnsAsync(availability);

            //Act
            var result = await _controller.Delete(id);

            //Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        [TestCase(1)]
        public async Task Delete_WithID_ShouldCallAvailabilitiesUpdate(int id)
        {
            //Arrange
            var availability = new Availability()
            {
                ID = 1,
                IsDeleted = false
            };
            _mockAvailabilities.Setup(r => r.GetById(id))
                .ReturnsAsync(availability);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockAvailabilities.Verify(r => r.Update(It.IsAny<Availability>()),
                                                             Times.Once);
        }


        [Test]
        [TestCase(1)]
        public async Task Delete_WithID_ShouldCallUnitOfWorkSaveChanges(int id)
        {
            //Arrange
            var availability = new Availability()
            {
                ID = 1,
                IsDeleted = false
            };
            _mockAvailabilities.Setup(r => r.GetById(id))
                .ReturnsAsync(availability);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChanges(), Times.Once);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Delete_WithInvalidID_ShouldNotCallAvailabilitiesGetById(int id)
        {
            //Act
            await _controller.Delete(id);

            //Assert
            _mockAvailabilities.Verify(r => r.GetById(id), Times.Never);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Delete_WithID_ShouldNotCallAvailabilitiesUpdate(int id)
        {
            //Arrange
            var availability = new Availability()
            {
                ID = 1,
                IsDeleted = false
            };
            _mockAvailabilities.Setup(r => r.GetById(id))
                .ReturnsAsync(availability);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockAvailabilities.Verify(r => r.Update(It.IsAny<Availability>()),
                                                             Times.Never);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Delete_WithInvalidID_ShouldNotCallUnitOfWorkSaveChanges(int id)
        {
            //Arrange
            var availability = new Availability()
            {
                ID = 1,
                IsDeleted = false
            };
            _mockAvailabilities.Setup(r => r.GetById(id))
                .ReturnsAsync(availability);

            //Act
            await _controller.Delete(id);

            //Assert
            _mockUnitOfWork.Verify(w => w.SaveChanges(), Times.Never);
        }

        [Test]
        [TestCase(1)]
        public async Task Delete_WithValidID_ShouldNotReturnBadRequestResultIfAvailabilityNotExist(int id)
        {
            //Arrange
            _mockAvailabilities.Setup(r => r.GetById(id))
                .ReturnsAsync(() => null);

            //Act
            var result = await _controller.Delete(id);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        #endregion
    }
}
