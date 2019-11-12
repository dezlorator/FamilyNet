using System;
using FamilyNetServer.Models;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class AvailabilityValidator : IAvailabilityValidator
    {
        public bool IsValid(AvailabilityDTO dto)
        {
            if (dto.StartTime < DateTime.Now ||
                dto.IsReserved == true ||
                dto.FreeHours.TotalMinutes < 30.0)
            {
                return false;
            }

            return true;
        }

        public bool IsOverlaping(AvailabilityDTO dto, Availability entity)
        {
           bool Overlaping = (dto.StartTime < entity.Date &&
                (dto.StartTime + dto.FreeHours) <= entity.Date) ||
                (dto.StartTime >= (entity.Date + entity.FreeHours) && 
                (dto.StartTime + dto.FreeHours) > (entity.Date + entity.FreeHours));

            return !Overlaping;
        }
    }
}
