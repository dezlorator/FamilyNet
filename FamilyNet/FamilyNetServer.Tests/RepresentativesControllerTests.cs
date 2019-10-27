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

namespace FamilyNetServer.Tests
{
    [TestFixture]
    class RepresentativesControllerTests
    {
        private Mock<IUnitOfWorkAsync> _mockUnitOfWork;
        private Mock<IFileUploader> _mockFileUploader;
        private Mock<IRepresentativeValidator> _mockValidator;
        private Mock<IFilterConditionsRepresentatives> _mockFilterConditions;
        private RepresentativesController _controller;
        private Mock<IAsyncRepository<Representative>> _mockRepresentatives;

        [SetUp]
        public virtual void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWorkAsync>();
            _mockFileUploader = new Mock<IFileUploader>();
            _mockFilterConditions = new Mock<IFilterConditionsRepresentatives>();
            _mockValidator = new Mock<IRepresentativeValidator>();
            _mockRepresentatives = new Mock<IAsyncRepository<Representative>>();
            _controller = new RepresentativesController(_mockFileUploader.Object,
                                                _mockUnitOfWork.Object,
                                                _mockValidator.Object,
                                                _mockFilterConditions.Object);
        }

        [Test]
        public void Create_GetAll_ReturnsListOfRepresentativesDTO()
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
            _mockUnitOfWork.Setup(t => t.Representatives).Returns(_mockRepresentatives.Object);

            //Act
            var result = _controller.GetAll(new FilterParametersRepresentatives())
                as IQueryable<RepresentativeDTO>;

            //Assert
            _mockRepresentatives.Verify(repo => repo.GetAll(), Times.Once);
        }
    }
}
