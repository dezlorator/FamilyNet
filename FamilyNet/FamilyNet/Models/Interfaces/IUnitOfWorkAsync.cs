namespace FamilyNet.Models.Interfaces
{
    public interface IUnitOfWorkAsync : IIdentityAsync
    {
        IOrphanageAsyncRepository Orphanages { get; }
        IAsyncRepository<Donation> Donations { get; }

        void SaveChangesAsync();
    }
}