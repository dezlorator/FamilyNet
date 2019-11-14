using FamilyNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;
using DataTransferObjects.Enums;

namespace FamilyNetServer.Validators
{
    public interface IFeedbackValidator : IPermissionFeedbackValidator
    {
        bool ValidateDTO(FeedbackDTO feedback, ref string errorMessage);
    }
}
