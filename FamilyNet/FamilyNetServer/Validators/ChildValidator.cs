using DataTransferObjects;
using Microsoft.Extensions.Logging;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildValidator : IChildValidator
    {
        #region fields

        private readonly ILogger<ChildValidator> _logger;

        #endregion

        #region ctor

        public ChildValidator(ILogger<ChildValidator> logger)
        {
            _logger = logger;
        }

        #endregion

        public bool IsValid(ChildDTO childDTO)
        {
            if (childDTO == null)
            {
                _logger.LogWarning("childDTO is null");
                return false;
            }

            var result = DateTimeIsPresent(childDTO.Birthday) &&
                   NameIsPresent(childDTO.Name) &&
                   SurnameIsPresent(childDTO.Surname) &&
                   PatronymicIsPresent(childDTO.Patronymic) &&
                   ChildrenHouseIsPresent(childDTO.ChildrenHouseID);
            _logger.LogInformation("ChildValidator returns " + result);

            return result;
        }

        private bool DateTimeIsPresent(DateTime birthday)
        {
            int maxChildAge = 19;
            var result = birthday >= DateTime.Now.AddYears(-maxChildAge);
            _logger.LogInformation("return " + result);

            return result;
        }

        private bool NameIsPresent(string name)
        {
            var result = !String.IsNullOrEmpty(name);
            _logger.LogInformation("return " + result);

            return result;
        }

        private bool SurnameIsPresent(string surname)
        {
            var result = !String.IsNullOrEmpty(surname);
            _logger.LogInformation("return " + result);

            return result;
        }

        private bool PatronymicIsPresent(string patronymic)
        {
            var result = !String.IsNullOrEmpty(patronymic);
            _logger.LogInformation("return " + result);

            return result;
        }

        private bool ChildrenHouseIsPresent(int childrenHouseId)
        {
            var result = childrenHouseId > 0;
            _logger.LogInformation("return " + result);

            return result;
        }
    }
}
