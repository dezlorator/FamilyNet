using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IChildActivityValidator
    {
        bool IsValid(ChildActivityDTO childrenActivityDTO);
    }
}
