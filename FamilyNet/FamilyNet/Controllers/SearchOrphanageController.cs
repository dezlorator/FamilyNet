using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Newtonsoft.Json;

namespace FamilyNet.Controllers {
    public class SearchOrphanageController : BaseController {
        public SearchOrphanageController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork) { }

        public async Task<IActionResult> Index() {

            var orphanages = _unitOfWorkAsync.Orphanages.GetAll();
            List<Orphanage> forOut= new List<Orphanage>();

            foreach (var item in orphanages) {
                if (item.MapCoordX != null && item.MapCoordY != null) 
                    {
                    forOut.Add(item);
                }
            }
            return View(forOut);
        }
    }
}