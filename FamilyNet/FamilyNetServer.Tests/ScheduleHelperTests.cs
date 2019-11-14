using System;
using DataTransferObjects;
using FamilyNetServer.Helpers;
using Moq;
using NUnit.Framework;

namespace FamilyNetServer.Tests
{
    [TestFixture]
    class ScheduleHelperTests
    {
        private IScheduleHelper _helper;

        [SetUp]
        public virtual void SetUp()
        {
            _helper = new ScheduleHelper(); 
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(-1, 6)]
        [TestCase(-3, 4)]
        public void AdjustDate_WhithValidAvailabilityDTO_ShouldReturnZero(int days, int expectedDaysToAdjust)
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                DayOfWeek = DateTime.Now.AddDays(days).DayOfWeek,
                StartTime = DateTime.Now,
            };

            //Act
            var result = _helper.AdjustDate(availabilityDTO);

            //Assert
            Assert.IsTrue(result == expectedDaysToAdjust);
        } 

        [Test]
        public void AdjustDate_WhithValidTodayAvailabilityDTO_ShouldReturnZero()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                DayOfWeek = DateTime.Now.DayOfWeek,
                StartTime = DateTime.Now + new TimeSpan(1, 0, 0),
            };

            //Act
            var result = _helper.AdjustDate(availabilityDTO);

            //Assert
            Assert.Zero(result);
        } 

        [Test]
        public void AdjustDate_WhitInValidTodayAvailabilityDTO_ShouldReturnSevenDaysInWeek()
        {
            //Arrange
            var availabilityDTO = new AvailabilityDTO()
            {
                DayOfWeek = DateTime.Now.DayOfWeek,
                StartTime = DateTime.Now - new TimeSpan(1, 0, 0),
            };
            var daysInWeek = 7;

            //Act
            var result = _helper.AdjustDate(availabilityDTO);

            //Assert
            Assert.IsTrue(result == daysInWeek);
        } 
    }
}
