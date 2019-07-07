using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.EntityFramework
{
    public class EFUnitOfWork : IUnitOfWork
    {
        #region Private fields

        private ApplicationDbContext _context;
        private EFRepository<Orphanage> _orphanagesRepository;
        private EFRepository<CharityMaker> _charityMakersRepository;
        private EFRepository<Representative> _representativeRepository;
        private EFRepository<Orphan> _orphansRepository;
        private EFRepository<Donation> _donationsRepository;
        private EFRepository<Volunteer> _volunteersRepository;

        #endregion

        #region Constructors

        public EFUnitOfWork(ApplicationDbContext cont)
        {
            _context = cont;
            _charityMakersRepository = new EFRepository<CharityMaker>(cont);
            _donationsRepository = new EFRepository<Donation>(cont);
            _orphanagesRepository = new EFRepository<Orphanage>(cont);
            _orphansRepository = new EFRepository<Orphan>(cont);
            _representativeRepository = new EFRepository<Representative>(cont);
            _volunteersRepository = new EFRepository<Volunteer>(cont);       
        }



        #endregion

        #region Property

        public IRepository<Orphanage> Orphanages => _orphanagesRepository;

        public IRepository<CharityMaker> CharityMakers => _charityMakersRepository;

        public IRepository<Representative> Representatives => _representativeRepository;

        public IRepository<Volunteer> Volunteers => _volunteersRepository;

        public IRepository<Donation> Donations => _donationsRepository;

        public IRepository<Orphan> Orphans => _orphansRepository;

        public void SaveChanges()
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
