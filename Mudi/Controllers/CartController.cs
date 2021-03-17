using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mudi_Models;
using Mudi_Utility;
using Mudi_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Mudi_DataAccess;
using Mudi_DataAccess.Repository.IRepository;

namespace Mudi.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _prodRepo;

        private readonly IWishListDetailRepository _wishDRepo;

        private readonly ICartRepository _cartRepo;

        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        [BindProperty]
        public ProductVM ProductVM { get; set; }

        public CartController(IApplicationUserRepository userRepo, IProductRepository prodRepo,
            IWishListDetailRepository wishDRepo, ICartRepository cartRepo,
            IOrderHeaderRepository orderHRepo, IOrderDetailRepository orderDRepo,
            IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {

            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _wishDRepo = wishDRepo;
            _orderDRepo = orderDRepo;
            _orderHRepo = orderHRepo;
            _cartRepo = cartRepo;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            // List<int> prodInCartSave = shoppingCartList.Select(i => i.ProductId).ToList();

            IEnumerable<Product> prodListTemp = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));
            IList<Product> prodList = new List<Product>();

            foreach (var cartObj in shoppingCartList)
            {
                //sending to db
                Cart cart = new Cart();
                cart.ApplicationUserId = claim.Value;
                cart.Qty = cartObj.Qty;
                cart.ProductId = cartObj.ProductId;
                var obj = _cartRepo.FirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == cart.ProductId);
                if (obj == null)
                {
                    _cartRepo.Add(cart);
                }
                else
                {

                }
                //sending to cartView

                Product prodTemp = prodListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempQty = cartObj.Qty;
                prodList.Add(prodTemp);
            }
            _cartRepo.Save();

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> ProdList)
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Qty = prod.TempQty });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value)
            };

            foreach (var cartObj in shoppingCartList)
            {
                Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempQty = cartObj.Qty;
                ProductUserVM.ProductList.Add(prodTemp);
            }



            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM ProductUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //we need to create an order

            OrderHeader orderHeader = new OrderHeader()
            {
                ApplicationUserId = claim.Value,
                FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.TempQty * x.Price),
                City = ProductUserVM.ApplicationUser.City,
                StreetAddress = ProductUserVM.ApplicationUser.StreetAddress,
                PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                FullName = ProductUserVM.ApplicationUser.FullName,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                OrderDate = DateTime.Now,
                OrderStatus = WC.StatusPending
            };
            _orderHRepo.Add(orderHeader);
            _orderHRepo.Save();

            foreach (var prod in ProductUserVM.ProductList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = orderHeader.Id,
                    PricePerUnit = prod.Price,
                    Qty = prod.TempQty,
                    ProductId = prod.Id
                };
                _orderDRepo.Add(orderDetail);

            }
            _orderDRepo.Save();
            TempData[WC.Success] = "Order is placed successfully";
            return RedirectToAction(nameof(OrderConfirmation), new { id = orderHeader.Id });




            //return RedirectToAction(nameof(InquiryConfirmation));

            //var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            //    + "templates" + Path.DirectorySeparatorChar.ToString() +
            //    "Inquiry.html";

            //var subject = "New Inquiry";
            //string HtmlBody = "";
            //using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            //{
            //    HtmlBody = sr.ReadToEnd();
            //}
            ////Name: { 0}
            ////Email: { 1}
            ////Phone: { 2}
            ////Products: {3}

            //StringBuilder productListSB = new StringBuilder();
            //foreach (var prod in ProductUserVM.ProductList)
            //{
            //    productListSB.Append($" - Name: { prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
            //}

            //string messageBody = string.Format(HtmlBody,
            //    ProductUserVM.ApplicationUser.FullName,
            //    ProductUserVM.ApplicationUser.Email,
            //    ProductUserVM.ApplicationUser.PhoneNumber,
            //    productListSB.ToString());

            //await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            //return RedirectToAction(nameof(InquiryConfirmation));

        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }
        public IActionResult Remove(int id)
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Qty = prod.TempQty });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
