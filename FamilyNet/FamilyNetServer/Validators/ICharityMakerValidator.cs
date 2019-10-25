using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface ICharityMakerValidator
    {
        bool IsValid(CharityMakerDTO charityMakerDTO);
    }
}
