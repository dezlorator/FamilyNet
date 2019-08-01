using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class DonationsController : BaseController
    {
        public DonationsController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {

        }

        // GET: Donations
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_unitOfWorkAsync.Donations.GetAll());
        }

        // GET: Donations/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityMaker = await _unitOfWorkAsync.CharityMakers.GetById((int)id);

            if (charityMaker == null)
            {
                return NotFound();
            }

            return View(charityMaker);
        }

        // GET: Donations/CreateRequest
        [Authorize(Roles = "Admin,CharityMaker,Volunteer,Representative")]
        public IActionResult CreateRequest()
        {
            return View();
        }

        // POST: Donations/CreateRequest
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CharityMaker,Volunteer")]
        public async Task<IActionResult> CreateRequest([Bind("ID,DonationItem")] Donation request)
        {          
            if (ModelState.IsValid)
            {
                request.IsRequest = true;                
                await _unitOfWorkAsync.Donations.Create(request);
                await _unitOfWorkAsync.Donations.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        // POST: Donations/CreateDonation
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CharityMaker,Volunteer")]
        public async Task<IActionResult> CreateDonation([Bind("ID,DonationItem")] Donation donation)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.Donations.Create(donation);
                await _unitOfWorkAsync.Donations.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(donation);
        }

        // GET: Donations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityMaker = await _unitOfWorkAsync.CharityMakers.GetById((int)id);

            if (charityMaker == null)
            {
                return NotFound();
            }

            return View(charityMaker);
        }

        // POST: Donations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id,
            [Bind("ID,DonationItem")] CharityMaker charityMaker)
        {
            if (id != charityMaker.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CharityMaker charityMakerToEdit = await _unitOfWorkAsync.CharityMakers.GetById(id);
                    charityMakerToEdit.CopyState(charityMaker);
                    _unitOfWorkAsync.CharityMakers.Update(charityMakerToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonationExists(charityMaker.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // TODO : logging
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(charityMaker);
        }

        // GET: Donations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Donation donation = await _unitOfWorkAsync.Donations
                .GetById((int)id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // POST: Orphans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphan = await _unitOfWorkAsync.Donations.GetById((int)id);
            if (orphan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            await _unitOfWorkAsync.Donations.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DonationExists(int id)
        {
            return _unitOfWorkAsync.Donations.GetById(id) != null;
        }
    }
}
