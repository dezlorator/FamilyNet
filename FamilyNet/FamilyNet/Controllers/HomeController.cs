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
        
        
        public HomeController(IUnitOfWorkAsync unitOfWork) : base ( unitOfWork)
        {
            
        }
       
        public async Task<IActionResult> Index()
        {
            
            await _unitOfWorkAsync.CharityMakers.Create(new CharityMaker()
            {
                FullName = new FullName() { Name = "33", Surname = "33", Patronymic = "3" },
                Address = new Adress() { City = "test2", Country = "test2", House = "test2", Region = "test2", Street = "test2" },
                Birthday = DateTime.Now,
                Contacts = new Contacts() { Email = "test2" },
                Donations = null,
                Rating = 2
            });
            _unitOfWorkAsync.SaveChangesAsync();
            var test = _unitOfWorkAsync.CharityMakers.GetAll().ToList();
            test[0].Address.City = "ooaoaooaotooa";
            await _unitOfWorkAsync.CharityMakers.Update(test[0]);
            _unitOfWorkAsync.SaveChangesAsync();
            var test2 = _unitOfWorkAsync.CharityMakers.GetAll().ToList();



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
