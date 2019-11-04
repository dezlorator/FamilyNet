using FamilyNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IFeedbackValidator
    {
        bool IsValid(FeedbackDTO feedback, ref string errorMessage);
    }
}
