using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;
using System;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class OrphanagesController : BaseController
    {
        #region Private fields

        private OrphanageSearchModel _searchModel;

        #endregion

        #region Ctor

        public OrphanagesController(IUnitOfWorkAsync unitOfWork)
            : base(unitOfWork)
        {
        }

        #endregion

        #region ActionMethods

        // GET: Orphanages
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, OrphanageSearchModel searchModel, 
            SortStateOrphanages sortOrder = SortStateOrphanages.NameAsc)
        {
            IQueryable<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll();

            orphanages = GetFiltered(orphanages, searchModel);
            SortBy(orphanages, sortOrder);

            if (id == 0)
                return View(await orphanages.ToListAsync());

            if (id > 0)
                orphanages = orphanages.Where(x => x.ID.Equals(id));

            return View(await orphanages.ToListAsync());
        }

        // GET: Orphanages/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // GET: Orphanages/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: Orphanages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Adress,Rating,Avatar")] Orphanage orphanage,
            IFormFile file) //TODO: AlPa -> Research Bind To Annotations
        {
            await ImageHelper.SetAvatar(orphanage, file, "wwwroot\\representatives");

            //part to add location when obj creating
            bool IsLocationNotNull = GetCoordProp(orphanage.Adress, out var Location);
            if (IsLocationNotNull)
            {
                orphanage.Location = new Location()
                {
                    MapCoordX = Location.Item1,
                    MapCoordY = Location.Item2
                };
            }
            else
                orphanage.LocationID = null;

            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.Orphanages.Create(orphanage);
                await _unitOfWorkAsync.Orphanages.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(orphanage);
        }

        // GET: Orphanages/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // POST: Orphanages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Representative")]
        public async Task<IActionResult> Edit([Bind("ID,Name,Adress,Rating,Avatar")]
            Orphanage orphanage, int id, IFormFile file) //TODO: Check change id position
        {
            if (id != orphanage.ID)
                return NotFound();

            await ImageHelper.SetAvatar(orphanage, file, "wwwroot\\representatives");

            if (ModelState.IsValid)
            {
                try
                {
                    //in ef to change the object you need to track it out of context
                    var orphanageToEdit = await _unitOfWorkAsync.Orphanages.GetById(orphanage.ID);

                    //copying the state with NOT CHANGING REFERENCES
                    orphanageToEdit.CopyState(orphanage);

                    //edit location
                    bool IsLocationNotNull = GetCoordProp(orphanage.Adress, out var Location);
                    if (IsLocationNotNull)
                    {
                        orphanageToEdit.Location = new Location()
                        {
                            MapCoordX = Location.Item1,
                            MapCoordY = Location.Item2
                        };
                    }
                    else
                        orphanageToEdit.LocationID = null;

                    _unitOfWorkAsync.Orphanages.Update(orphanageToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.Orphanages.Any(orphanage.ID))
                        return NotFound();
                    else
                        throw; //TODO: Loging
                }

                return RedirectToAction(nameof(Index));
            }

            return View(orphanage);
        }

        // GET: Orphanages/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // POST: Orphanages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
            await _unitOfWorkAsync.Orphanages.Delete(orphanage.ID);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult SearchByTypeHelp() => View();

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SearchResult(string typeHelp)
        {
            ViewData["TypeHelp"] = typeHelp;
            var list = _unitOfWorkAsync.Orphanages.Get(
                orp => orp.Donations.Where(
                    donat => donat.DonationItem.DonationItemTypes.Where(
                        donatitem => donatitem.Name.ToLower().Contains(typeHelp.ToLower())).Count() > 0
                        && donat.IsRequest).
                    Count() > 0);

            return View("SearchResult", list);
        }

        [AllowAnonymous]
        public IActionResult SearchOrphanageOnMap()
        {
            var orphanages = _unitOfWorkAsync.Orphanages.GetForSearchOrphanageOnMap();

            return View(orphanages);
        }

        #endregion

        #region Private Helpers

        private bool Contains(Address addr)
        {
            foreach (var word in _searchModel.AddressString.Split())
            {
                string wordUpper = word.ToUpper();

                if (addr.Street.ToUpper().Contains(wordUpper)
                        || addr.City.ToUpper().Contains(wordUpper)
                        || addr.Region.ToUpper().Contains(wordUpper)
                        || addr.Country.ToUpper().Contains(wordUpper))
                    return true;
            }

            return false;
        }

        private void SortBy(IQueryable<Orphanage> orphanages, SortStateOrphanages sortOrder)
        {
            ViewData["NameSort"] = sortOrder == SortStateOrphanages.NameAsc
                ? SortStateOrphanages.NameDesc : SortStateOrphanages.NameAsc;
            ViewData["AddressSort"] = sortOrder == SortStateOrphanages.AddressAsc
                ? SortStateOrphanages.AddressDesc : SortStateOrphanages.AddressAsc;
            ViewData["RatingSort"] = sortOrder == SortStateOrphanages.RatingAsc
                ? SortStateOrphanages.RatingDesc : SortStateOrphanages.RatingAsc;

            switch (sortOrder)
            {
                case SortStateOrphanages.NameDesc:
                    orphanages = orphanages.OrderByDescending(s => s.Name);
                    break;
                case SortStateOrphanages.AddressAsc:
                    orphanages = orphanages
                        .OrderBy(s => s.Adress.Country)
                        .ThenBy(s => s.Adress.Region)
                        .ThenBy(s => s.Adress.City)
                        .ThenBy(s => s.Adress.Street);
                    break;
                case SortStateOrphanages.AddressDesc:
                    orphanages = orphanages
                        .OrderByDescending(s => s.Adress.Country)
                        .ThenByDescending(s => s.Adress.Region)
                        .ThenByDescending(s => s.Adress.City)
                        .ThenByDescending(s => s.Adress.Street);
                    break;
                case SortStateOrphanages.RatingAsc:
                    orphanages = orphanages.OrderBy(s => s.Rating);
                    break;
                case SortStateOrphanages.RatingDesc:
                    orphanages = orphanages.OrderByDescending(s => s.Rating);
                    break;
                default:
                    orphanages = orphanages.OrderBy(s => s.Name);
                    break;
            }
        }

        private IQueryable<Orphanage> GetFiltered(IQueryable<Orphanage> orphanages,
            OrphanageSearchModel searchModel)
        {
            if (searchModel != null)
            {
                _searchModel = searchModel;

                if (!string.IsNullOrEmpty(searchModel.NameString))
                    orphanages = orphanages.Where(x => x.Name.Contains(searchModel.NameString));

                if (!string.IsNullOrEmpty(searchModel.AddressString))
                    orphanages = orphanages.Where(x => Contains(x.Adress));

                if (searchModel.RatingNumber > 0)
                    orphanages = orphanages.Where(x => x.Rating >= searchModel.RatingNumber);
            }

            return orphanages;
        }

        private bool GetCoordProp(Address address, out Tuple<float?, float?> result)
        {
            result = null;
            bool forOut = false;

            var nominatim = new Nominatim.API.Geocoders.ForwardGeocoder();
            var d = nominatim.Geocode(new Nominatim.API.Models.ForwardGeocodeRequest()
            {
                Country = address.Country,
                State = address.Region,
                City = address.City,
                StreetAddress = String.Concat(address.Street, " ", address.House)
            });

            //TODO:some validation for search
            if (d.Result.Count() != 0)
            {
                float? X = (float)d.Result[0].Latitude;
                float? Y = (float)d.Result[0].Longitude;

                result = new Tuple<float?, float?>(X, Y);
                forOut = true;
            }

            return forOut;
        }
		#endregion
    }
}
