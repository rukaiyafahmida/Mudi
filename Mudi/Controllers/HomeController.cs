using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mudi_Models;
using Mudi_Utility;
using Mudi_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using System.IO;
using System.Text;
using Mudi_DataAccess;
using Mudi_DataAccess.Repository.IRepository;

namespace Mudi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        private readonly IProductRepository _prodRepo;
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly ICategoryRepository _catRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IWishListDetailRepository _wishDRepo;
        private readonly ICartRepository _cartRepo;


        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        [BindProperty]
        public ContactUsVM ContactUsVM { get; set; }
        public OrderListVM OrderListVM { get; set; }

        public HomeController(ILogger<HomeController> logger, IProductRepository prodRepo,
            ICategoryRepository catRepo, IApplicationUserRepository userRepo,
            IOrderHeaderRepository orderHRepo, ICartRepository cartRepo,
            IWishListDetailRepository wishDRepo,
            IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _logger = logger;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _catRepo = catRepo;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _orderHRepo = orderHRepo;
            _cartRepo = cartRepo;
            _wishDRepo = wishDRepo;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            HomeVM homeVM = new HomeVM()
            {
                Products = _prodRepo.GetAll(includeProperties: "Category"),
                Categories = _catRepo.GetAll(),
                OrderHList = _orderHRepo.GetAll()

        };
            if (User.IsInRole(WC.CustomerRole))
            {
                homeVM.CartList = _cartRepo.GetAll(u => u.ApplicationUserId == claim.Value);

                List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
                
                foreach (var item in homeVM.CartList)
                {
                    var obj = shoppingCartList.FirstOrDefault(u => u.ProductId == item.ProductId);
                    if (obj == null)
                    {
                        //cart item doesnot exist in session add it to session
                        shoppingCartList.Add(new ShoppingCart { ProductId = item.ProductId, Qty = item.Qty });
                        HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
                    }
                }
                return View(homeVM);
            }
            else
                return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = _prodRepo.FirstOrDefault(u => u.Id == id, includeProperties: "Category"),
                ExistsInCart = false

            };


            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    DetailsVM.ExistsInCart = true;
                }
            }

            //homeVM.WishListItems = _wishDRepo.GetAll(u => u.ApplicationUserId == claim.Value);
            //foreach (var item in )
            //{
            //    if (item.ProductId == id)
            //    {
            //        DetailsVM.ExistsInCart = true;
            //    }
            //}

            return View(DetailsVM);
        }
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //if there are products in the session and user is not logged in 
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id, Qty = detailsVM.Product.TempQty });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            TempData[WC.Success] = "Item add to cart successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            TempData[WC.Success] = "Item removed from cart successfully";
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ContactUs()
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            //var userId = User.FindFirstValue(ClaimTypes.Name);
            ContactUsVM = new ContactUsVM();
            if (claim != null)
                ContactUsVM.ApplicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value);

            return View(ContactUsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ContactUs")]
        public async Task<IActionResult> ContactUsPost(ContactUsVM ContactUsVM)
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "ContactMail.html";

            var subject = ContactUsVM.Subject;
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            //Name: { 0}
            //Email: { 1}
            //Phone: { 2}
            //Message: {3}

            StringBuilder message = new StringBuilder();

            message.Append(ContactUsVM.Message);


            string messageBody = string.Format(HtmlBody,
                ContactUsVM.ApplicationUser.FullName,
                ContactUsVM.ApplicationUser.Email,
                ContactUsVM.ApplicationUser.PhoneNumber,
                message.ToString());

            await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult AboutUs()
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
