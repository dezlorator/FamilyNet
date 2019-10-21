﻿using FamilyNetServer.Infrastructure;
using FamilyNetServer.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.Interfaces
{
    public interface IUnitOfWork : IIdentityAsync
    {
        IAsyncRepository<Address> Address { get; }
        IAsyncRepository<Location> Location { get; }
        IOrphanageAsyncRepository Orphanages { get; }
        IAsyncRepository<CharityMaker> CharityMakers { get; }
        IAsyncRepository<Representative> Representatives { get; }
        IAsyncRepository<Volunteer> Volunteers { get; }
        IAsyncRepository<Donation> Donations { get; }
        IAsyncRepository<Orphan> Orphans { get; }
        IAsyncRepository<BaseItemType> BaseItemTypes { get; }
        IAsyncRepository<DonationItem> DonationItems { get; }

        DbSet<TypeBaseItem> TypeBaseItems { get; set; }// TODO : rewrite this

        void SaveChangesAsync();
    }
}