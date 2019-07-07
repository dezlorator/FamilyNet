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
        private EFRepositoryAsync<Orphanage> _orphanagesRepository;
        private EFRepositoryAsync<CharityMaker> _charityMakersRepository;
        private EFRepositoryAsync<Representative> _representativeRepository;
        private EFRepositoryAsync<Orphan> _orphansRepository;
        private EFRepositoryAsync<Donation> _donationsRepository;
        private EFRepositoryAsync<Volunteer> _volunteersRepository;

        #endregion

        #region Constructors

        public EFUnitOfWorkAsync(ApplicationDbContext cont)
        {
            _context = cont;
            _charityMakersRepository = new EFRepositoryAsync<CharityMaker>(cont);
            _donationsRepository = new EFRepositoryAsync<Donation>(cont);
            _orphanagesRepository = new EFRepositoryAsync<Orphanage>(cont);
            _orphansRepository = new EFRepositoryAsync<Orphan>(cont);
            _representativeRepository = new EFRepositoryAsync<Representative>(cont);
            _volunteersRepository = new EFRepositoryAsync<Volunteer>(cont);
        }



        #endregion

        #region Property

        public IAsyncRepository<Orphanage> Orphanages => _orphanagesRepository;

        public IAsyncRepository<CharityMaker> CharityMakers => _charityMakersRepository;

        public IAsyncRepository<Representative> Representatives => _representativeRepository;

        public IAsyncRepository<Volunteer> Volunteers => _volunteersRepository;

        public IAsyncRepository<Donation> Donations => _donationsRepository;

        public IAsyncRepository<Orphan> Orphans => _orphansRepository;

        public void SaveChangesAsync()
        {
            _context.SaveChanges();
            //OR 
            //_orphanagesRepository.SaveChanges();
            //_charityMakersRepository.SaveChanges();
            //_representativeRepository.SaveChanges();
            //_volunteersRepository.SaveChanges();
            //_donationsRepository.SaveChanges();
            //_orphansRepository.SaveChanges();
        }

        #endregion
    }
}
