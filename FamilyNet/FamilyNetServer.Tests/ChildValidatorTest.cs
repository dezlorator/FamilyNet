using DataTransferObjects;
using FamilyNetServer.Validators;
using NUnit.Framework;

namespace FamilyNetServer.Tests
{
    class ChildValidatorTest
    {
        [Test]
        public void ChildValidator_WithNullChildDTO_ReturnFalse()
        {
            var validator = new ChildValidator();

            Assert.False(validator.IsValid(null));
        }

        [Test]
        public void ChildValidator_WithEmptyChildDTO_ReturnFalse()
        {
            var childDTO = new ChildDTO();

            var validator = new ChildValidator();

            Assert.False(validator.IsValid(childDTO));
        }
    }
}
