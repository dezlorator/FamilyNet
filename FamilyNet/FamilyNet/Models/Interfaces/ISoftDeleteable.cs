namespace FamilyNet.Models.Interfaces
{
    public interface ISoftDeleteable
    {
        bool IsDeleted { get; set; }
    }
}