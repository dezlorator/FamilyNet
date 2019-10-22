
namespace FamilyNetServer.Validators
{
    public interface IValidator<T> where T : class
    {
        bool IsValid(T objDTO);
    }
}
