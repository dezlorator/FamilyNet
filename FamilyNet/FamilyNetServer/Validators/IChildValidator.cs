using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IChildValidator
    {
        bool IsValid(ChildDTO childDTO);
    }
}
