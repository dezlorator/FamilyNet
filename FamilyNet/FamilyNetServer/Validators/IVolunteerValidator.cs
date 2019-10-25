using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IVolunteerValidator
    {
        bool IsValid(VolunteerDTO volunteerDTO);
    }
}
