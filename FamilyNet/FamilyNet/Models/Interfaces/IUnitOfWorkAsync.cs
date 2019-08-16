using FamilyNet.Infrastructure;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        IAsyncRepository<BaseItemType> BaseItemTypes { get; }

        DbSet<TypeBaseItem> TypeBaseItems { get; set; }// TODO : rewrite this

        void SaveChangesAsync();
    }
}