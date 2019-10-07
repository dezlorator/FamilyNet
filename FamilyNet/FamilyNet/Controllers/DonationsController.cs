using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace FamilyNet.Controllers
{
    ///контроллер донатов
    [Authorize]
    public class DonationsController : BaseController
    {
        /// <summary>
        /// Конструктор BaseController.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public DonationsController(IUnitOfWorkAsync unitOfWork)
            : base(unitOfWork)
        {

        }

        // GET: DonationsTable
        /// <summary>
        /// Передает на интерфейс таблицу донатов. Доступно для всех.
        /// </summary>
        [AllowAnonymous]
        public IActionResult DonationsTable()
        {
            return View(_unitOfWorkAsync.Donations.GetAll());
        }

        // GET: CategoriesTable
        /// <summary>
        /// Передает на интерфейс таблицу категорий. Доступно для всех.
        /// </summary>
        [AllowAnonymous]
        public IActionResult CategoriesTable()
        {
            return View(_unitOfWorkAsync.BaseItemTypes.GetAll());
        }

        #region CreateCategory

        // GET: DonationsTable/CreateCategory
        /// <summary>
        /// Создание категории(GET). Доступно для Admin,CharityMaker,Volunteer,Representative.
        /// </summary>
        [Authorize(Roles = "Admin,CharityMaker,Volunteer,Representative")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: Donations/CreateCategory
        /// <summary>
        /// Создание категории(POST). Доступно для Admin,CharityMaker,Volunteer.
        /// </summary>
        /// <param name="request">Свойства ID,Name</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CharityMaker,Volunteer")]
        public async Task<IActionResult> CreateCategory([Bind("ID,Name")] DonationItemType request)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.BaseItemTypes.Create(request);
                await _unitOfWorkAsync.BaseItemTypes.SaveChangesAsync();
                return RedirectToAction(nameof(CategoriesTable));
            }
            return View(request);
        }

        #endregion

        #region DeleteCategory

        // GET: CategoriesTable/Delete/id
        /// <summary>
        /// Удаление категории по id(GET). Доступно для Admin.
        /// </summary>
        /// <param name="id">id категории</param>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BaseItemType category = await _unitOfWorkAsync.BaseItemTypes
                .GetById((int)id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: CategoriesTable/Delete/id
        /// <summary>
        /// Подтверждение удаления категории по id(GET). Доступно для Admin.
        /// </summary>
        /// <param name="id">id категории</param>
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _unitOfWorkAsync.BaseItemTypes.GetById((int)id);
            if (category == null)
            {
                return RedirectToAction(nameof(CategoriesTable));
            }
            await _unitOfWorkAsync.BaseItemTypes.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(CategoriesTable));
        }

        #endregion

        #region CreateDonation

        // GET: DonationsTable/CreateRequest
        /// <summary>
        /// Просмотр таблицы потребностей. Доступен для Admin,CharityMaker,Volunteer,Representative.
        /// </summary>
        [Authorize(Roles = "Admin,CharityMaker,Volunteer,Representative")]
        public IActionResult CreateDonation()
        {
            ViewBag.ListOfOrphanages = _unitOfWorkAsync.Orphanages.GetAll();
            ViewBag.ListOfBaseItemTypes = _unitOfWorkAsync.BaseItemTypes.GetAll();
            return View();
        }

        // POST: DonationsTable/CreateRequest
        /// <summary>
        /// Создать потребность. Доступен для Admin,CharityMaker,Volunteer.
        /// </summary>
        /// <param name="request">Свойства ID,DonationItem,Orphanage</param>
        /// <param name="idOrphanage">id приюта</param>
        /// <param name="idDonationItem">id пожертвования</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CharityMaker,Volunteer")]
        public async Task<IActionResult> CreateDonation([Bind("ID,DonationItem,Orphanage")] Donation request,
                                                        int idOrphanage, int idDonationItem)
        {
            if (ModelState.IsValid)
            {
                request.OrphanageID = idOrphanage;

                request.IsRequest = true;

                await _unitOfWorkAsync.Donations.Create(request);
                await _unitOfWorkAsync.Donations.SaveChangesAsync();

                _unitOfWorkAsync.TypeBaseItems.Add(new TypeBaseItem()
                {
                    ItemID = request.DonationItem.ID,
                    TypeID = idDonationItem
                });

                _unitOfWorkAsync.SaveChangesAsync();

                return RedirectToAction(nameof(DonationsTable));
            }
            return View(request);
        }

        #endregion      

        #region EditDonation

        // GET: Donations/Edit/5
        /// <summary>
        /// Изменение пожертвования(GET). Доступно для Admin.
        /// </summary>
        /// <param name="id">id пожертвования для изменения</param>
        /// <returns>NotFound или View</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditDonation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.ListOfOrphanages = _unitOfWorkAsync.Orphanages.GetAll();
            ViewBag.ListOfBaseItemTypes = _unitOfWorkAsync.BaseItemTypes.GetAll();
            var donation = await _unitOfWorkAsync.Donations.GetById((int)id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // POST: Donations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Изменение пожертвования(Post).  Доступно для Admin.
        /// </summary>
        /// <param name="id">id пожертвования</param>
        /// <param name="donation">свойства ID,DonationItem</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditDonation(int id,
            [Bind("ID,DonationItem")] Donation donation)
        {
            if (id != donation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Donation donationToEdit = await _unitOfWorkAsync.Donations.GetById(id);
                    donationToEdit.CopyState(donation);
                    _unitOfWorkAsync.Donations.Update(donationToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonationExists(donation.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // TODO : logging
                    }
                }
                return RedirectToAction(nameof(DonationsTable));
            }
            return View(donation);
        }

        #endregion

        #region DeleteDonation

        // GET: DonationsTable/Delete/id
        /// <summary>
        /// Удаление пожертования по id(GET). Доступно для Admin.
        /// </summary>
        /// <param name="id">id пожертвования</param>
        /// <returns>NotFound или View</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonation(int? id)
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

        // POST: DonationsTable/Delete/id
        /// <summary>
        /// Подтвержение удаления по id. Доступо для Admin.
        /// </summary>
        /// <param name="id">id пожертвования</param>
        [HttpPost, ActionName("DeleteDonation")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonationConfirmed(int id)
        {
            var donation = await _unitOfWorkAsync.Donations.GetById((int)id);
            if (donation == null)
            {
                return RedirectToAction(nameof(DonationsTable));
            }
            await _unitOfWorkAsync.Donations.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(DonationsTable));
        }

        #endregion

        #region DetailsDonation

        // GET: Donations/Details/id
        /// <summary>
        /// Получить детали пожертвования по id. Доступно для всех. 
        /// </summary>
        /// <param name="id">id пожертвования</param>
        /// <returns>View(donation) или NotFound</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donation = await _unitOfWorkAsync.Donations.GetById((int)id);
            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        #endregion

        #region DetailsCategory
        /// <summary>
        /// Получить детали категории по id.
        /// </summary>
        /// <param name="id">id категории</param>
        /// <returns>View(category) или NotFound</returns>
        public async Task<IActionResult> DetailsCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWorkAsync.BaseItemTypes.GetById((int)id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        #endregion

        private bool DonationExists(int id)
        {
            return _unitOfWorkAsync.Donations.GetById(id) != null;
        }

        private bool CategoryExists(int id)
        {
            return _unitOfWorkAsync.BaseItemTypes.GetById(id) != null;
        }
    }
}