using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using DataTransferObjects;
using FamilyNet.Downloader;
using Microsoft.Extensions.Localization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace FamilyNet.Controllers
{
    public class CategoriesController : BaseController
    {

        #region private fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStringLocalizer<CategoriesController> _localizer;
        private readonly ServerSimpleDataDownloader<CategoryDTO> _downloader;
        private readonly IURLCategoriesBuilder _URLBuilder;
        private readonly string _apiPath = "api/v1/categories";

        #endregion

        #region ctor

        public CategoriesController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<CategoriesController> localizer,
                                 ServerSimpleDataDownloader<CategoryDTO> downloader,
                                 IURLCategoriesBuilder uRLBuilder)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _downloader = downloader;
            _URLBuilder = uRLBuilder;
        }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var url = _URLBuilder.GetAll(_apiPath);
            IEnumerable<CategoryDTO> categoryDTO = null;

            try
            {
                categoryDTO = await _downloader.GetAllAsync(url);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            var categories = categoryDTO.Select(category => new BaseItemType()
            {
                ID = category.ID,
                Name = category.Name
            });

            return View(categories);
        }

        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create()
        {
            await Check();
            GetViewData();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create(CategoryDTO category)
        {
            var url = _URLBuilder.CreatePost(_apiPath);
            var msg = await _downloader.CreatePostAsync(url, category);

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return Redirect("/Categories/Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLBuilder.GetById(_apiPath, id.Value);
            CategoryDTO categoryDTO;

            try
            {
                categoryDTO = await _downloader.GetByIdAsync(url);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            if (categoryDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(categoryDTO);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _URLBuilder.GetById(_apiPath, id);
            var msg = await _downloader.DeleteAsync(url);

            if (msg.StatusCode != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Categories/Index");
        }

        private void GetViewData()
        {
            ViewData["CategoriesList"] = _localizer["CategoriesList"];
        }
    }
}