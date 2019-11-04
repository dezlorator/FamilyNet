using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class FeedbackValidator : IFeedbackValidator
    {
        #region private
        private const string EMPTY_MESSAGE = "Message field is empty";
        private const string TIME_NOT_SET = "Time field is null";
        private const string RATING_IS_OUT_OF_RANGE = "Rating must be bigger than -10 and lesser than 10";
        #endregion
        public bool IsValid(FeedbackDTO feedback, ref string errorMessage)
        {
            if(string.IsNullOrEmpty(feedback.Message))
            {
                errorMessage = EMPTY_MESSAGE;
                return false;
            }
            if(feedback.Time == null)
            {
                errorMessage = TIME_NOT_SET;
                return false;
            }
            if(feedback.Rating > 10 || feedback.Rating < -10)
            {
                errorMessage = RATING_IS_OUT_OF_RANGE;
                return false;
            }
            return true;
        }
    }
}
