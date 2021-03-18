using Microsoft.AspNetCore.Mvc;
using Mudi_DataAccess.Repository.IRepository;
using Mudi_Models;
using Mudi_Models.ViewModels;
using Mudi_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mudi.Controllers
{
    public class WebSiteDetailController : Controller
    {
        private readonly IWebSiteDetailRepository _webDRepo;

        [BindProperty]
        public WebSiteDetailVM WebSiteDetailVM { get; set; }

        public WebSiteDetailController(
        IWebSiteDetailRepository webDRepo)
        {
            _webDRepo = webDRepo;

        }



        public IActionResult Index()
        {
            IEnumerable<WebSiteDetail> objList = _webDRepo.GetAll();
            return View(objList);
        }

        

        //GET - EDIT
        public IActionResult EditAboutUs(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _webDRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAboutUs()
        {
            //if (ModelState.IsValid)
            {
                

                //_webDRepo.Update(obj);
                _webDRepo.Save();
                TempData[WC.Success] = " About us Edited successfully";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "Error while editing category";
            //return View(obj);

        }
        //GET - EDIT
        public IActionResult EditContactUs(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _webDRepo.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditContactUs()
        {
            //if (ModelState.IsValid)
            {
                var obj = _webDRepo.FirstOrDefault(u => u.Id == WebSiteDetailVM.Id);
                obj.ContactUs = WebSiteDetailVM.ContactUs;
                _webDRepo.Update(obj);
                _webDRepo.Save();
                TempData[WC.Success] = "Contact Us Edited successfully";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "Error while editing category";
            //return View(obj);

        }

    }
}