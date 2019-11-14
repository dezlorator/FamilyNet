using System;
using Microsoft.Extensions.Logging;
using FamilyNetServer.Models;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class AvailabilityValidator : IAvailabilityValidator
    {
        #region fields

        private readonly ILogger<AvailabilityValidator> _logger;

        #endregion

        #region ctor

        public AvailabilityValidator(ILogger<AvailabilityValidator> logger)
        {
            _logger = logger;
        }

        #endregion

        public bool IsValid(AvailabilityDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("AvailabilityDTO is null");
                return false;
            }

            if (IsCorrectStartTime(dto) &&
                IsNotReserved(dto) &&
                IsFreeTimeEnough(dto))
            {
                return true;
            }

            return false;
        }

        public bool IsCorrectStartTime(AvailabilityDTO dto)
        {
            var result = DateTime.Now < dto.StartTime;
            _logger.LogInformation("return " + result);

            return result;
        }

        public bool IsNotReserved (AvailabilityDTO dto)
        {
            var result = !dto.IsReserved;
            _logger.LogInformation("return " + result);

            return result;
        }

        public bool IsFreeTimeEnough (AvailabilityDTO dto)
        {
            var result = !(dto.FreeHours.TotalMinutes < 30.0);
            _logger.LogInformation("return " + result);

            return result;
        }

        public bool IsOverlaping(AvailabilityDTO dto, Availability entity)
        {
            if (dto == null)
            {
                _logger.LogWarning("AvailabilityDTO is null");
                return false;
            }
            
            if (entity == null)
            {
                _logger.LogWarning("Availability is null");
                return false;
            }

            bool Overlaping = (dto.StartTime < entity.Date &&
                (dto.StartTime + dto.FreeHours) <= entity.Date) ||
                (dto.StartTime >= (entity.Date + entity.FreeHours) && 
                (dto.StartTime + dto.FreeHours) > (entity.Date + entity.FreeHours));

            return !Overlaping;
        }
    }
}
