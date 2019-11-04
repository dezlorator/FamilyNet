using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IChildrenActivityValidator
    {
        bool IsValid(ChildrenActivityDTO childrenActivityDTO);
    }
}
