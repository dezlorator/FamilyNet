using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;

namespace FamilyNet.Controllers
{
    public class HomeController : BaseController
    {
        const int bestOfThree = 3;
        
        public HomeController(IUnitOfWorkAsync unitOfWork) : base ( unitOfWork)
        {
            
        }
       
        public async Task<IActionResult> Index()
        {
            ViewData["Best"] = _unitOfWorkAsync.Orphanages.GetAll()
              .OrderByDescending(c => c.Rating)
              .Take(bestOfThree);
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
