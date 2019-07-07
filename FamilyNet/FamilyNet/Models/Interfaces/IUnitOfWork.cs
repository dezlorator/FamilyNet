using System.Threading.Tasks;

namespace FamilyNet.Models.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Orphanage> Orphanages { get; }
        IRepository<CharityMaker> CharityMakers { get; }
        IRepository<Representative> Representatives { get; }
        IRepository<Volunteer> Volunteers { get; }
        IRepository<Donation> Donations { get; }
        IRepository<Orphan> Orphans { get; }

        void SaveChanges();
    }
}