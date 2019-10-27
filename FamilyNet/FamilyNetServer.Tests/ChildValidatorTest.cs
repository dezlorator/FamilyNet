using DataTransferObjects;
using FamilyNetServer.Validators;
using NUnit.Framework;
using System;

namespace FamilyNetServer.Tests
{
    class ChildValidatorTest
    {
        private IChildValidator _validator;

        [SetUp]
        public void Init()
        {
            _validator = new ChildValidator();
        }

        [Test]
        public void ChildValidator_WithNullChildDTO_ReturnFalse()
        {
            Assert.False(_validator.IsValid(null));
        }

        [Test]
        public void ChildValidator_WithEmptyChildDTO_ReturnFalse()
        {
            Assert.False(_validator.IsValid(new ChildDTO()));
        }


        [Test]
        public void ChildValidator_WithEmptyNameChildDTO_ReturnFalse()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new DateTime(2011, 5, 30),
                ChildrenHouseID = 2,
                Patronymic = "Петровна",
                Surname = "Левчкеко"
            };

            Assert.False(_validator.IsValid(childDTO));
        }


        [Test]
        public void ChildValidator_WithEmptySurnameChildDTO_ReturnFalse()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new DateTime(2011, 5, 30),
                ChildrenHouseID = 2,
                Patronymic = "Петровна",
                Name = "Инна"
            };

            Assert.False(_validator.IsValid(childDTO));
        }

        [Test]
        public void ChildValidator_WithEmptyPatronamicChildDTO_ReturnFalse()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new DateTime(2011, 5, 30),
                ChildrenHouseID = 2,
                Surname = "Петрова",
                Name = "Инна"
            };

            Assert.False(_validator.IsValid(childDTO));
        }

        [Test]
        public void ChildValidator_WithNullChildrenHousePatronamicChildDTO_ReturnFalse()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new DateTime(2011, 5, 30),
                Surname = "Петрова",
                Name = "Инна",
                Patronymic = "Егоровна"
            };

            Assert.False(_validator.IsValid(childDTO));
        }

        [Test]
        public void ChildValidator_WithEmptyBirthdayChildDTO_ReturnFalse()
        {
            var childDTO = new ChildDTO()
            {
                Surname = "Петрова",
                Name = "Инна",
                Patronymic = "Егоровна",
                ChildrenHouseID = 2
            };

            Assert.False(_validator.IsValid(childDTO));
        }

        [Test]
        public void ChildValidator_WithValidChildDTO_ReturnTrue()
        {
            var childDTO = new ChildDTO()
            {
                Birthday = new DateTime(2011, 5, 5),
                Surname = "Петрова",
                Name = "Инна",
                Patronymic = "Егоровна",
                ChildrenHouseID = 2
            };

            Assert.True(_validator.IsValid(childDTO));
        }
    }
}
