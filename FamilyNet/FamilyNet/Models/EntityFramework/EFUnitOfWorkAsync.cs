using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.EntityFramework
{
    public class EFUnitOfWorkAsync : IUnitOfWorkAsync
    {
        #region Private fields

        private ApplicationDbContext _context;
       
        #endregion

        #region Constructors

        public EFUnitOfWorkAsync(ApplicationDbContext cont)
        {
            _context = cont;
            CharityMakers = new EFRepositoryAsync<CharityMaker>(cont);
            Donations = new EFRepositoryAsync<Donation>(cont);
            Orphanages = new EFRepositoryAsync<Orphanage>(cont);
            Orphans = new EFRepositoryAsync<Orphan>(cont);
            Representatives = new EFRepositoryAsync<Representative>(cont);
            Volunteers = new EFRepositoryAsync<Volunteer>(cont);
        }



        #endregion

        #region Property

        public IAsyncRepository<Orphanage> Orphanages { get; set; }
        public IAsyncRepository<CharityMaker> CharityMakers { get; set; }

        public IAsyncRepository<Representative> Representatives { get; set; }

        public IAsyncRepository<Volunteer> Volunteers { get; set; }

        public IAsyncRepository<Donation> Donations { get; set; }

        public IAsyncRepository<Orphan> Orphans { get; set; }

        public void SaveChangesAsync()
        {
            _context.SaveChanges();
        }

        #endregion
    }
}
