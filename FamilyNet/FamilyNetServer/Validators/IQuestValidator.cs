using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IQuestValidator
    {
        bool IsValid(QuestDTO questDTO);
    }
}
