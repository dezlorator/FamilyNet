namespace FamilyNet.Models.Interfaces
{
    public interface IEntity : ISoftDeleteable
    {
        int ID { get; set; }
    }
}