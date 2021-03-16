using Microsoft.AspNetCore.Mvc;
using Mudi_DataAccess.Repository.IRepository;
using System;
using Mudi_Models;
using Mudi_Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mudi_Utility;
using System.Security.Claims;

namespace Mudi.Controllers
{
    public class WishListController : Controller
    {

        [BindProperty]
        public WishListVM WishListVM { get; set; }

        private readonly IWishListHedaerRepository _wishHRepo;
        private readonly IWishListDetailRepository _wishDRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _prodRepo;



        public WishListController(IWishListHedaerRepository wishHRepo, IWishListDetailRepository wishDRepo
            , IProductRepository prodRepo, IApplicationUserRepository userRepo)
        {
            _wishDRepo = wishDRepo;
            _wishHRepo = wishHRepo;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
       // [HttpPost]
        public IActionResult Add(int id)

        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            WishListDetail obj = new WishListDetail();
            obj=_wishDRepo.FirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == id);
            if(obj == null) //product is not in wishlist of the user
            {
                WishListDetail obj1 = new WishListDetail();
                obj1.ApplicationUserId = claim.Value;
                obj1.ProductId = id;
                _wishDRepo.Add(obj1);
                _wishDRepo.Save();
                TempData[WC.Success] = "Added to WishList successfully";
                return RedirectToAction("Index");
            }
            

            //else //already has a wishlist
            //{

            //}


            //if (ModelState.IsValid)
            //{
            //    _wishDRepo.Add(obj);
            //    _wishDRepo.Save();
            //    TempData[WC.Success] = "Added to WishList successfully";
            //    return RedirectToAction("Index");
            //}
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index");
        }
    }
}
