using Microsoft.AspNetCore.Mvc;
using Mudi_DataAccess.Repository.IRepository;
using System;
using Mudi_Models;
using Mudi_Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mudi.Controllers
{
    public class WishListController : Controller
    {

        [BindProperty]
        public WishListVM WishListVM { get; set; }

        private readonly IWishListHedaerRepository _wishHRepo;
        private readonly IWishListDetailRepository _wishDRepo;

        public WishListController(IWishListHedaerRepository wishHRepo, IWishListDetailRepository wishDRepo)
        {
            _wishDRepo = wishDRepo;
            _wishHRepo = wishHRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddProductToWishList(int id)
        {

            return RedirectToAction(nameof(Index));
        }
    }
}
