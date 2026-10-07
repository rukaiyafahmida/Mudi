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

            IEnumerable<Cart> carts = _cartRepo.GetAll(u => u.ApplicationUserId == claim.Value);
            foreach (var cartDB in carts)
            {
                if(!shoppingCartList.Exists(x => x.ProductId == cartDB.ProductId))
                    shoppingCartList.Add(new ShoppingCart { ProductId = cartDB.ProductId, Qty = cartDB.Qty });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);


            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            // List<int> prodInCartSave = shoppingCartList.Select(i => i.ProductId).ToList();

            IEnumerable<Product> prodListTemp = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));
            IList<Product> prodList = new List<Product>();

            foreach (var cartObj in shoppingCartList)
            {
                //sending to db
                Cart cart = new Cart
                {
                    ApplicationUserId = claim.Value,
                    Qty = cartObj.Qty,
                    ProductId = cartObj.ProductId
                };
                var obj = _cartRepo.FirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == cart.ProductId);
                if (obj == null)
                {
                    _cartRepo.Add(cart);
                }
                
                //sending to cartView
                Product prodTemp = prodListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);
                if (prodTemp == null) continue;
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList ?? Enumerable.Empty<Product>())
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Qty = prod.TempQty });
                var obj = _cartRepo.FirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == prod.Id);
                var product = _prodRepo.Find(prod.Id);
                if (obj == null || product == null || prod.TempQty < 1 || prod.TempQty > 100 || prod.TempQty > product.Stock)
                    return BadRequest("Invalid cart quantity.");
                obj.Qty = prod.TempQty;
                _cartRepo.Update(obj);
            }
            _cartRepo.Save();
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
                if (prodTemp == null) continue;
                prodTemp.TempQty = cartObj.Qty;
                ProductUserVM.ProductList.Add(prodTemp);
            }

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserVM ProductUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cartItems = _cartRepo.GetAll(u => u.ApplicationUserId == claim.Value).ToList();
            if (cartItems.Count == 0) return RedirectToAction(nameof(Index));
            var products = new List<Product>();
            foreach (var cart in cartItems)
            {
                var product = _prodRepo.Find(cart.ProductId);
                if (product == null || cart.Qty < 1 || cart.Qty > 100 || cart.Qty > product.Stock)
                    return BadRequest("Your cart contains unavailable items or quantities.");
                product.TempQty = cart.Qty;
                products.Add(product);
            }
            // Read prices and quantities from the saved cart rather than hidden form inputs.
            ProductUserVM.ProductList = products;
            var customer = ProductUserVM.ApplicationUser;
            if (customer == null || new[] { customer.FullName, customer.PhoneNumber,
                customer.StreetAddress, customer.City, customer.PostalCode }.Any(string.IsNullOrWhiteSpace))
            {
                ModelState.AddModelError("", "Please complete the delivery details.");
                return View("Summary", ProductUserVM);
            }

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

            foreach (var prod in ProductUserVM.ProductList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeader = orderHeader,
                    PricePerUnit = prod.Price,
                    Qty = prod.TempQty,
                    ProductId = prod.Id
                };
                var prodPopularity = _prodRepo.FirstOrDefault(u => u.Id == prod.Id);
                prodPopularity.ProductPopularity++;
                _prodRepo.Update(prodPopularity);
                _orderDRepo.Add(orderDetail);
             }
            _orderDRepo.Save();
            TempData[WC.Success] = "Order is placed successfully";
            return RedirectToAction(nameof(OrderConfirmation), new { id = orderHeader.Id });


        }
        public IActionResult OrderConfirmation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id && u.ApplicationUserId == userId);
            if (orderHeader == null) return NotFound();
            //after oder is done the cart is cleared

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Cart> carts = _cartRepo.GetAll(u => u.ApplicationUserId == claim.Value);
            _cartRepo.RemoveRange(carts);
            _cartRepo.Save();


            HttpContext.Session.Clear();
            return View(orderHeader);
        }
        public IActionResult Remove(int id)
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

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));

            var obj = _cartRepo.FirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == id);
            if (obj != null) _cartRepo.Remove(obj);
            _cartRepo.Save();

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> ProdList)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList ?? Enumerable.Empty<Product>())
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Qty = prod.TempQty });
                var obj = _cartRepo.FirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == prod.Id);
                var product = _prodRepo.Find(prod.Id);
                if (obj == null || product == null || prod.TempQty < 1 || prod.TempQty > 100 || prod.TempQty > product.Stock)
                    return BadRequest("Invalid cart quantity.");
                obj.Qty = prod.TempQty;
                _cartRepo.Update(obj);
            }
            _cartRepo.Save();

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Clear()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Cart> carts = _cartRepo.GetAll(u => u.ApplicationUserId == claim.Value);
            _cartRepo.RemoveRange(carts);
            _cartRepo.Save();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
