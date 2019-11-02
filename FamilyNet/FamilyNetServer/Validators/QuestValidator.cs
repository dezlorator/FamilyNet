using System;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class QuestValidator : IQuestValidator
    {
        public bool IsValid(QuestDTO questDTO)
        {
            return (!String.IsNullOrEmpty(questDTO.Name) &&
                    questDTO.DonationID == null || questDTO.DonationID > 0 &&
                    questDTO.VolunteerID == null || questDTO.VolunteerID > 0);
        }
    }
}
