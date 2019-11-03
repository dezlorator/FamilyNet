using Microsoft.EntityFrameworkCore;

namespace FamilyNetServer.Models.Interfaces
{
    public interface IUnitOfWork : IIdentity
    {
        IRepository<Address> Address { get; }
        IRepository<Location> Location { get; }
        IOrphanageRepository Orphanages { get; }
        IRepository<CharityMaker> CharityMakers { get; }
        IRepository<Representative> Representatives { get; }
        IRepository<Volunteer> Volunteers { get; }
        IRepository<Donation> Donations { get; }
        IRepository<Orphan> Orphans { get; }
        IRepository<BaseItemType> BaseItemTypes { get; }
        IRepository<DonationItem> DonationItems { get; }
        IRepository<Quest> Quests { get; set; }
        IRepository<Availability> Availabilities { get; }

        DbSet<TypeBaseItem> TypeBaseItems { get; set; }// TODO : rewrite this

        void SaveChangesAsync();
    }
}