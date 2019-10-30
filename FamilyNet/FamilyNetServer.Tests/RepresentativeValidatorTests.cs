using DataTransferObjects;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyNetServer.Tests
{
    class RepresentativeValidatorTests
    {
        private RepresentativeValidator _validator;
        private RepresentativeDTO _representativeDTO;

        [SetUp]
        public void SetUp()
        {
            _validator = new RepresentativeValidator();
            _representativeDTO = new RepresentativeDTO
            {
                Avatar = new Mock<IFormFile>().Object,
                EmailID = 1,
                Patronymic = "Иванович",
                ID = 1,
                PhotoPath = It.IsAny<string>(),
                Rating = 3.2f
            };
        }

        [Test]
        public void IsValid_WithValidRepresentativeDTO_ShouldReturnTrue()
        {
            //Arrange
            _representativeDTO.Name = "Иван";
            _representativeDTO.Surname = "Иванов";
            _representativeDTO.Birthday = new DateTime(1990, 1, 1);
            _representativeDTO.ChildrenHouseID = 1;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValid_WithEmptyName_ShouldReturnFalse()
        {
            //Arrange
            _representativeDTO.Name = "";
            _representativeDTO.Surname = "Иванов";
            _representativeDTO.Birthday = new DateTime(1990, 1, 1);
            _representativeDTO.ChildrenHouseID = 1;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_WithNullName_ShouldReturnFalse()
        {
            //Arrange
            _representativeDTO.Name = null;
            _representativeDTO.Surname = "Иванов";
            _representativeDTO.Birthday = new DateTime(1990, 1, 1);
            _representativeDTO.ChildrenHouseID = 1;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsValid_WithEmptySurname_ShouldReturnFalse()
        {
            //Arrange
            _representativeDTO.Name = "Иван";
            _representativeDTO.Surname = "";
            _representativeDTO.Birthday = new DateTime(1990, 1, 1);
            _representativeDTO.ChildrenHouseID = 1;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsValid_WithNullSurname_ShouldReturnFalse()
        {
            //Arrange
            _representativeDTO.Name = "Иван";
            _representativeDTO.Surname = null;
            _representativeDTO.Birthday = new DateTime(1990, 1, 1);
            _representativeDTO.ChildrenHouseID = 1;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_WithEmptyBirthday_ShouldReturnFalse()
        {
            //Arrange
            _representativeDTO.Name = "Иван";
            _representativeDTO.Surname = "Иванов";
            _representativeDTO.Birthday = new DateTime();
            _representativeDTO.ChildrenHouseID = 1;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsValid_WithInvalidBirthday_ShouldReturnFalse()
        {
            //Arrange
            _representativeDTO.Name = "Иван";
            _representativeDTO.Surname = "Иванов";
            _representativeDTO.Birthday = new DateTime(1800, 1, 1);
            _representativeDTO.ChildrenHouseID = 1;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsFalse(result);
        }
            
        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void IsValid_WithInvalidChildrenHouseID_ShouldReturnFalse(int id)
        {
            //Arrange
            _representativeDTO.Name = "Иван";
            _representativeDTO.Surname = "Иванов";
            _representativeDTO.Birthday = new DateTime(1800, 1, 1);
            _representativeDTO.ChildrenHouseID = id;

            //Act
            var result = _validator.IsValid(_representativeDTO);

            //Assert
            Assert.IsFalse(result);
        }

    }
}
