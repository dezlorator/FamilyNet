using DataTransferObjects.Enums;

namespace FamilyNetServer.Validators
{
    public interface IPermissionFeedbackValidator
    {
        bool CheckPermission(UserRole sender, UserRole receiver);
    }
}
