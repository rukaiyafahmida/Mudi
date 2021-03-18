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

        public WebSiteDetailController(IWebSiteDetailRepository webDRepo)
        {
            _webDRepo = webDRepo;

        }



        public IActionResult Index()
        {
            IEnumerable<WebSiteDetail> objList = _webDRepo.GetAll();
            return View(objList);
        }

        

        //GET - EDIT
        public IActionResult EditAboutUs(int id)
        {
            var obj = _webDRepo.FirstOrDefault(u => u.Id == id);

            WebSiteDetailVM = new WebSiteDetailVM();
            WebSiteDetailVM.WebSiteDetailId = id;
            WebSiteDetailVM.AboutUs = obj.AboutUs;
            WebSiteDetailVM.ContactUs = obj.ContactUs;
            return View(WebSiteDetailVM);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAboutUsPost(WebSiteDetailVM WebSiteDetailVM)
        {
            var obj = _webDRepo.FirstOrDefault(u => u.Id == WebSiteDetailVM.WebSiteDetailId);
            obj.AboutUs = WebSiteDetailVM.AboutUs;
            _webDRepo.Update(obj);
            _webDRepo.Save();
            TempData[WC.Success] = "About Us Edited successfully";
            return RedirectToAction("Index");

            TempData[WC.Error] = "Error while editing category";
            //return View(obj);

        }
        //GET - EDIT
        public IActionResult EditContactUs(int id)
        {
            
            var obj = _webDRepo.FirstOrDefault(u => u.Id == id);

            WebSiteDetailVM = new WebSiteDetailVM();
            WebSiteDetailVM.WebSiteDetailId = id;
            WebSiteDetailVM.ContactUs = obj.ContactUs;
            WebSiteDetailVM.AboutUs = obj.AboutUs;
            return View(WebSiteDetailVM);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditContactUsPost()
        {
                
                var obj = _webDRepo.FirstOrDefault(u => u.Id == WebSiteDetailVM.WebSiteDetailId);
                obj.ContactUs = WebSiteDetailVM.ContactUs;
                _webDRepo.Update(obj);
                _webDRepo.Save();
                TempData[WC.Success] = "Contact Us Edited successfully";
                return RedirectToAction("Index");
            
            TempData[WC.Error] = "Error while editing category";
            //return View(obj);

        }

    }
}