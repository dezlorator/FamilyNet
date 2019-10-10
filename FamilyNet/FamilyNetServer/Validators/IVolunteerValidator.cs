using FamilyNetServer.DTO;

namespace FamilyNetServer.Validators
{
    public interface IVolunteerValidator
    {
        bool IsValid(VolunteerDTO volunteerDTO);
    }
}
