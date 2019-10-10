using FamilyNetServer.DTO;

namespace FamilyNetServer.Validators
{
    public interface IChildValidator
    {
        bool IsValid(ChildDTO childDTO);
    }
}
