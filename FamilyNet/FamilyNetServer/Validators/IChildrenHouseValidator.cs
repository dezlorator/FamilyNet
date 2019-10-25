using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IChildrenHouseValidator
    {
        bool IsValid(ChildrenHouseDTO childrenHouseDTO);
    }
}
