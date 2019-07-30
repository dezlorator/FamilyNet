using FamilyNet.Infrastructure;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace FamilyNet.Models.Interfaces
{
    public interface IUnitOfWorkAsync : IIdentityAsync
    {
        IOrphanageAsyncRepository Orphanages { get; }
        IAsyncRepository<CharityMaker> CharityMakers { get; }
        IAsyncRepository<Representative> Representatives { get; }
        IAsyncRepository<Volunteer> Volunteers { get; }
        IAsyncRepository<Donation> Donations { get; }
        IAsyncRepository<Orphan> Orphans { get; }
        void SaveChangesAsync();
    }
}