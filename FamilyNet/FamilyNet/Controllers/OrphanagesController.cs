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
using Microsoft.Extensions.Localization;
using System.Globalization;
using FamilyNet.Infrastructure;
using System;
using System.Collections.Generic;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class OrphanagesController : BaseController
    {
        #region Private fields

        //первая инициализация в методе index
        /// <summary>
        /// Поисковая модель
        /// </summary>
        private OrphanageSearchModel _searchModel;

        //это кажется не надо. Не используется в этом контроллере
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Хранит данные, связанные с локализацией
        /// </summary>
        private readonly IStringLocalizer<OrphanagesController> _localizer;

        public OrphanagesController(IUnitOfWorkAsync unitOfWork, IHostingEnvironment environment,
            IStringLocalizer<OrphanagesController> localizer, IStringLocalizer<SharedResource> sharedLocalizer)
            : base(unitOfWork, sharedLocalizer)
        {
            _hostingEnvironment = environment;
            _localizer = localizer;
        }

        //это нигде не используется
        private bool IsContain(Address addr)
        {
            foreach (var word in _searchModel.AddressString.Split())
            {
                if (addr.Street.ToUpper().Contains(word.ToUpper())
                || addr.City.ToUpper().Contains(word.ToUpper())
                || addr.Region.ToUpper().Contains(word.ToUpper())
                || addr.Country.ToUpper().Contains(word.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ActionMethods

        // GET: Orphanages
        /// <summary>
        /// Получает список всех приютов по указанному id
        /// </summary>
        /// <param name="id">Уникальный id приюта</param>
        /// <param name="searchModel">Поисковая модель</param>
        /// <param name="sortOrder">Параметр сортировки</param>
        /// <returns>Возращает объект представления с параметром IQueryable<Orphanage></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, OrphanageSearchModel searchModel,
            SortStateOrphanages sortOrder = SortStateOrphanages.NameAsc)
        {
            IQueryable<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll();

            //выполняем фильтрацию
            orphanages = GetFiltered(orphanages, searchModel);
            //сортируем
            orphanages = GetSorted(orphanages, sortOrder);

            //выводим много много много абсолютно все приюты
            if (id == 0)
                // можно убрать ToListAsync()
                return View(await orphanages.ToListAsync());

            //выводим только один приют
            if (id > 0)
                orphanages = orphanages.Where(x => x.ID.Equals(id));

            // можно убрать ToListAsync() ????
            return View(await orphanages.ToListAsync());
        }


        // GET: Orphanages/Details/5
        /// <summary>
        /// Отображает полное состояние приюта, указанного по id
        /// </summary>
        /// <param name="id">Уникальный id приюта</param>
        /// <returns>Возращает объект представления с параметром Orphanage</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();
            GetViewData();

            return View(orphanage);
        }

        // GET: Orphanages/Create
        /// <summary>
        /// Отображает форму для ввода данных приюта
        /// </summary>
        /// <returns>Возращает объект представления</returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: Orphanages/Create
        /// <summary>
        /// Добавляет нового Orphanage в БД
        /// </summary>
        /// <param name="orphanage">Добавляемый приют</param>
        /// <param name="file">Объект загрузки файла на сервер</param>
        /// <returns>Возращает объект представления</returns>
        [HttpPost]
        //для валидации
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Adress,Rating,Avatar")] Orphanage orphanage,
            IFormFile file) //TODO: AlPa -> Research Bind To Annotations
        {
            //file нужен для загрузки файлов на сервер
            //установка аватара
            await ImageHelper.SetAvatar(orphanage, file, "wwwroot\\avatars");

            //вычисляем координаты по введенным данным
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

            //если все ок, то добавляем в бд и возвращемся в главный метод
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.Orphanages.Create(orphanage);
                await _unitOfWorkAsync.Orphanages.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            //если не ок, то вводим данные заново
            return View(orphanage);
        }

        // GET: Orphanages/Edit/5
        /// <summary>
        /// Выводит форму для редактирования конкретного приюта, указанного по id
        /// </summary>
        /// <param name="id">Уникальный id приюта</param>
        /// <returns>Возращает объект представления</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();
            GetViewData();

            return View(orphanage);
        }

        // POST: Orphanages/Edit/5
        /// <summary>
        /// Редактирует конкретный приют, указанный по id
        /// </summary>
        /// <param name="orphanage">Объект, что будет редактироваться</param>
        /// <param name="id">Уникальный id приюта</param>
        /// <param name="file">Объект загрузки файла на сервер</param>
        /// <returns>Возращает объект представления</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Representative")]
        public async Task<IActionResult> Edit([Bind("ID,Name,Adress,Rating,Avatar")]
            Orphanage orphanage, int id, IFormFile file) //TODO: Check change id position
        {
            if (id != orphanage.ID)
                return NotFound();

            await ImageHelper.SetAvatar(orphanage, file, "wwwroot\\avatars");

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
            GetViewData();

            return View(orphanage);
        }

        // GET: Orphanages/Delete/5
        /// <summary>
        /// Выводит форму для удаления конкретного приюта по указанному id
        /// </summary>
        /// <param name="id">Уникальный id приюта</param>
        /// <returns>Возращает объект представления</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();
            GetViewData();

            return View(orphanage);
        }

        // POST: Orphanages/Delete/5
        /// <summary>
        /// Удаляет выбранный по id приют из БД
        /// </summary>
        /// <param name="id">Уникальный id приюта</param>
        /// <returns>Возращает объект представления</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
            await _unitOfWorkAsync.Orphanages.Delete(orphanage.ID);
            _unitOfWorkAsync.SaveChangesAsync();
            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Выводит форму для поиска приюта по виду помощи
        /// </summary>
        /// <returns>Возращает обьект представления</returns>
        [AllowAnonymous]
        public IActionResult SearchByTypeHelp()
        {
            GetViewData();
            return View();
        }

        /// <summary>
        /// Выполняет поиск приюта по введенному виду помощи
        /// </summary>
        /// <returns>Возращает объект представления со списом приютов </returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult SearchResult(string typeHelp)
        {
            ViewData["TypeHelp"] = typeHelp;
            IEnumerable<Orphanage> list = new List<Orphanage>();
            if (typeHelp != null)
                list = _unitOfWorkAsync.Orphanages.Get(
                    orp => orp.Donations.Where(
                        donat => donat.DonationItem.TypeBaseItem.Where(
                            donatitem => donatitem.Type.Name.ToLower().Contains(typeHelp.ToLower())).
                            Count() > 0 && donat.IsRequest).
                        Count() > 0);
            GetViewData();

            return View("SearchResult", list);
        }

        /// <summary>
        /// Показывает карту с приютами
        /// </summary>
        /// <returns>Возращает объект представления со всеми приютами</returns>
        [AllowAnonymous]
        public IActionResult SearchOrphanageOnMap()
        {
            var orphanages = _unitOfWorkAsync.Orphanages.GetForSearchOrphanageOnMap();
            GetViewData();

            return View(orphanages);
        }


        #endregion

        #region Private Helpers

        /// <summary>
        /// Содержит ли Address указанную строку
        /// </summary>
        /// <param name="addr">Полный формат адреса</param>
        /// <returns>Возращает true, если содержит</returns>
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

        /// <summary>
        /// Выполняет сортировку приютов по указанному критерию
        /// </summary>
        /// <param name="orphanages">Список приютов</param>
        /// <param name="sortOrder">Критерий сортировки</param>
        /// <returns>Возращает отсортированный список приютов</returns>
        private IQueryable<Orphanage> GetSorted(IQueryable<Orphanage> orphanages, SortStateOrphanages sortOrder)
        {
            //данные для ui
            ViewData["NameSort"] = sortOrder == SortStateOrphanages.NameAsc
                ? SortStateOrphanages.NameDesc : SortStateOrphanages.NameAsc;
            ViewData["AddressSort"] = sortOrder == SortStateOrphanages.AddressAsc
                ? SortStateOrphanages.AddressDesc : SortStateOrphanages.AddressAsc;
            ViewData["RatingSort"] = sortOrder == SortStateOrphanages.RatingAsc
                ? SortStateOrphanages.RatingDesc : SortStateOrphanages.RatingAsc;

            //в зависимости от состояния выпоняем сортировку
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

            return orphanages;
        }

        /// <summary>
        /// Выолняет выборку приютов по заданному фильтру
        /// </summary>
        /// <param name="orphanages">Список приютов</param>
        /// <param name="searchModel">Фильтр для выборки приютов</param>
        /// <returns>Возращает список приютов</returns>
        private IQueryable<Orphanage> GetFiltered(IQueryable<Orphanage> orphanages,
            OrphanageSearchModel searchModel)
        {
            if (searchModel != null)
            {
                _searchModel = searchModel;

                //ищем дет дома по указ имени
                if (!string.IsNullOrEmpty(searchModel.NameString))
                    orphanages = orphanages.Where(x => x.Name.Contains(searchModel.NameString));

                //ищем дет дома по указ адресу
                if (!string.IsNullOrEmpty(searchModel.AddressString))
                    orphanages = orphanages.Where(x => Contains(x.Adress));

                //ищем по рейтингу
                if (searchModel.RatingNumber > 0)
                    orphanages = orphanages.Where(x => x.Rating >= searchModel.RatingNumber);
            }
            GetViewData();

            return orphanages;
        }

        /// <summary>
        /// Вычисляет координаты по указанному адресу
        /// </summary>
        /// <param name="address">Адресс для вычисления</param>
        /// <param name="result">Кортеж с координатами</param>
        /// <returns>Возращает true, если удалось вычислить координаты</returns>
        private bool GetCoordProp(Address address, out Tuple<float?, float?> result)
        {
            result = null;
            bool forOut = false;

            //геокодирование !?
            var nominatim = new Nominatim.API.Geocoders.ForwardGeocoder();
            //формируем геоданные
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

        /// <summary>
        /// Инициализирует ViewData и передает данные локализации на ui
        /// </summary>
        private void GetViewData()
        {
            ViewData["Name"] = _localizer["Name"];
            ViewData["Rating"] = _localizer["Rating"];
            ViewData["Photo"] = _localizer["Photo"];
            ViewData["Actions"] = _localizer["Actions"];
            ViewData["SaveChanges"] = _localizer["SaveChanges"];

            ViewData["ReturnToList"] = _localizer["ReturnToList"];
            ViewData["Details"] = _localizer["Details"];
            ViewData["Profile"] = _localizer["Profile"];
            ViewData["Address"] = _localizer["Address"];
            ViewData["From"] = _localizer["From"];
            @ViewData["ListOrphanages"] = _localizer["ListOrphanages"];
        }

        #endregion
    }
}

