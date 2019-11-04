using FamilyNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Validators
{
    public interface IFeedbackValidator
    {
        bool IsValid(Feedback feedback, ref string errorMessage);
    }
}
