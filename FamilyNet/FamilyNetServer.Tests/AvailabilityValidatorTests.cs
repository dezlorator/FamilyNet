using System;
using Microsoft.Extensions.Logging;
using FamilyNetServer.Models;
using FamilyNetServer.Validators;
using DataTransferObjects;
using Moq;
using NUnit.Framework;

namespace FamilyNetServer.Tests
{
    [TestFixture]
    class AvailabilityValidatorTests
    {
        private Mock<ILogger<AvailabilityValidator>> _mockLogger;
        private IAvailabilityValidator _validator;

        [SetUp]
        public virtual void SetUp()
        {
            _mockLogger = new Mock<ILogger<AvailabilityValidator>>();
            _validator = new AvailabilityValidator(_mockLogger.Object);
        }

        [Test]
        public void IsValid_WhithValidAvailabilityDTO_ShouldReturnTrue()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(1),
                FreeHours = new TimeSpan(1, 0, 0)
            };

            //Act
            var result = _validator.IsValid(availabilityDTO);

            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsValid_WhithInvalidDate_ShouldReturnFalse()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(-1),
                FreeHours = new TimeSpan(1, 0, 0)
            };

            //Act
            var result = _validator.IsValid(availabilityDTO);

            //Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsValid_WhithInvalidAvailabilityDTO_ShouldReturnFalse()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(1),
                FreeHours = new TimeSpan(1, 0, 0),
                IsReserved = true
            };

            //Act
            var result = _validator.IsValid(availabilityDTO);

            //Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsValid_WhithSmallDuration_ShouldReturnFalse()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(1),
                FreeHours = new TimeSpan(0, 20, 0),
                IsReserved = true
            };

            //Act
            var result = _validator.IsValid(availabilityDTO);

            //Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsValid_WhithNull_ShouldReturnFalse()
        {
            //Arrange
            AvailabilityDTO availabilityDTO = null;

            //Act
            var result = _validator.IsValid(availabilityDTO);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsOverlapping_WhithFullOverlaping_ShouldReturnTrue()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(2).AddHours(1),
                FreeHours = new TimeSpan(1, 0, 0),
            };
            var availabilitiy = new Availability()
            {
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
            };

            //Act
            var result = _validator.IsOverlaping(availabilityDTO, availabilitiy);

            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsOverlapping_WhithFullOuterOverlaping_ShouldReturnTrue()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(4, 0, 0),
            };
            var availabilitiy = new Availability()
            {
                Date = DateTime.Now.AddDays(2).AddHours(2),
                FreeHours = new TimeSpan(1, 0, 0),
            };

            //Act
            var result = _validator.IsOverlaping(availabilityDTO, availabilitiy);

            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsOverlapping_WhithUpperTimeLimitOverlaping_ShouldReturnTrue()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(2).AddHours(1),
                FreeHours = new TimeSpan(1, 0, 0),
            };
            var availabilitiy = new Availability()
            {
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
            };

            //Act
            var result = _validator.IsOverlaping(availabilityDTO, availabilitiy);

            //Assert
            Assert.IsTrue(result);
        }
         
        [Test]
        public void IsOverlapping_WhithLowerTimeLimitOverlaping_ShouldReturnTrue()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(2).AddHours(-1),
                FreeHours = new TimeSpan(3, 0, 0),
            };
            var availabilitiy = new Availability()
            {
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(1, 0, 0),
            };

            //Act
            var result = _validator.IsOverlaping(availabilityDTO, availabilitiy);

            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsOverlapping_WhithDateBefore_ShouldReturnFalse()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(3, 0, 0),
            };
            var availabilitiy = new Availability()
            {
                Date = DateTime.Now.AddDays(3),
                FreeHours = new TimeSpan(1, 0, 0),
            };

            //Act
            var result = _validator.IsOverlaping(availabilityDTO, availabilitiy);

            //Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsOverlapping_WhithDateAfter_ShouldReturnFalse()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                StartTime = DateTime.Now.AddDays(3),
                FreeHours = new TimeSpan(2, 0, 0),
            };
            var availabilitiy = new Availability()
            {
                Date = DateTime.Now.AddDays(2),
                FreeHours = new TimeSpan(1, 0, 0),
            };

            //Act
            var result = _validator.IsOverlaping(availabilityDTO, availabilitiy);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
