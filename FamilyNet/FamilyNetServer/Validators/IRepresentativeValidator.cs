
using FamilyNetServer.DTO;

namespace FamilyNetServer.Validators
{
    public interface IRepresentativeValidator
    {
        bool IsValid(RepresentativeDTO representativeDTO);
    }
}
