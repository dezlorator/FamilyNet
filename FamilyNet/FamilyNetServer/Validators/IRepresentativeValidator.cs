using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IRepresentativeValidator
    {
        bool IsValid(RepresentativeDTO representativeDTO);
    }
}
