using System;
using DataTransferObjects;
using Microsoft.Extensions.Logging;

namespace FamilyNetServer.Validators
{
    public class RepresentativeValidator : IRepresentativeValidator
    {
        #region fields

        private readonly ILogger<RepresentativeValidator> _logger;

        #endregion

        #region ctor

        public RepresentativeValidator(ILogger<RepresentativeValidator> logger)
        {
            _logger = logger;
        }

        #endregion

        public bool IsValid(RepresentativeDTO representativeDTO)
        {
            if (representativeDTO == null)
            {
                _logger.LogWarning("childDTO is null");
                return false;
            }

            var result = !(isValidDate(representativeDTO.Birthday) ||
                String.IsNullOrEmpty(representativeDTO.Name) ||
                String.IsNullOrEmpty(representativeDTO.Surname) ||
                representativeDTO.ChildrenHouseID < 0);

            _logger.LogInformation("RepresentativeValidator returns " + result);

            return result;
        }

        private bool isValidDate(DateTime birthday)
        {
            var maxAge = 90;

            var result = !(birthday.Year > (DateTime.Now.Year - maxAge));
            _logger.LogInformation("return " + result);

            return result;
        }
    }
}