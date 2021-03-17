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
using Microsoft.AspNetCore.Authorization;

namespace Mudi.Controllers
{
    public class WishListController : Controller
    {

        [BindProperty]
        public WishListVM WishListVM { get; set; }

        private readonly IWishListDetailRepository _wishDRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _prodRepo;



        public WishListController(IWishListDetailRepository wishDRepo
            , IProductRepository prodRepo, IApplicationUserRepository userRepo)
        {
            _wishDRepo = wishDRepo;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
        }


        [Authorize]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var objList = _wishDRepo.GetAll(u => u.ApplicationUserId == claim.Value).Join(_prodRepo.GetAll(),
                                                                                             w => w.ProductId,
                                                                                             p => p.Id,
                                                                                             (wish, product) => new
                                                                                             {
                                                                                                 wishListId= wish.Id,
                                                                                                 productName = product.Name,
                                                                                                 productPrice = product.Price,
                                                                                                 productStock=product.Stock,
                                                                                                 productId = product.Id
                                                                         
                                                                                             });
            List<WishListVM> WishListVM = new List<WishListVM>();

            foreach (var details in objList)
            {
                WishListVM wishList = new WishListVM()
                {
                    Id= details.wishListId,
                    ProductId = details.productId,
                    ProductName=details.productName,
                    UnitPrice=details.productPrice,
                    Stock=details.productStock
                };
                WishListVM.Add(wishList);
            }
            return View(WishListVM);  
        }

       [Authorize]
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
              
            }
            else
            {
                TempData[WC.Info] = "Already In WishList";
            }
            
        return RedirectToAction("Index");
        }
        
        public IActionResult Delete(int? id)
        {
            var obj = _wishDRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            _wishDRepo.Remove(obj);
            _wishDRepo.Save();
            TempData[WC.Success] = "Product Removed From WishList";
            return RedirectToAction("Index");


        }
    }
}
