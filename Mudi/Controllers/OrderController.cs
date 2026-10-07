using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;
        private readonly IProductRepository _prodRepo;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        [BindProperty]
        public ProductVM ProductVM { get; set; }


        public OrderController(
        IOrderHeaderRepository orderHRepo, IOrderDetailRepository orderDRepo, IProductRepository prodRepo)
        {
            _orderDRepo = orderDRepo;
            _orderHRepo = orderHRepo;
            _prodRepo = prodRepo;
        }


        [Authorize(Roles = WC.AdminRole)]
        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _orderHRepo.GetAll(),
                StatusList = WC.listStatus.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }

            return View(orderListVM);
        }
        [Authorize(Roles = WC.AdminRole)]
        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id),
                OrderDetail = _orderDRepo.GetAll(o => o.OrderHeaderId == id, includeProperties: "Product")
            };

            if (OrderVM.OrderHeader == null) return NotFound();
            return View(OrderVM);
        }
        public IActionResult DetailsUser(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id && u.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)),
                OrderDetail = _orderDRepo.GetAll(o => o.OrderHeaderId == id, includeProperties: "Product")
            };

            if (OrderVM.OrderHeader == null) return NotFound();
            return View(OrderVM);
        }
        public IActionResult IndexUser()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _orderHRepo.GetAll(u => u.ApplicationUserId == claim.Value)
            };
            return View(orderListVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = WC.AdminRole)]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null) return NotFound();
            if (orderHeader.OrderStatus != WC.StatusPending)
                return BadRequest("Only pending orders can start processing.");
            orderHeader.OrderStatus = WC.StatusInProcess;
            _orderHRepo.Save();
            TempData[WC.Success] = "Action completed successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = WC.AdminRole)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null) return NotFound();
            if (orderHeader.OrderStatus != WC.StatusInProcess)
                return BadRequest("Only processing orders can be shipped.");
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHRepo.Save();
            TempData[WC.Success] = "Action completed successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = WC.AdminRole)]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null) return NotFound();
            if (orderHeader.OrderStatus == WC.StatusCompleted || orderHeader.OrderStatus == WC.StatusCancelled)
                return BadRequest("This order cannot be cancelled.");
            orderHeader.OrderStatus = WC.StatusCancelled;
            _orderHRepo.Save();
            TempData[WC.Success] = "Action completed successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = WC.AdminRole)]
        public IActionResult CompleteOrder(OrderVM orderVM)
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null) return NotFound();
            if (orderHeader.OrderStatus != WC.StatusShipped)
                return BadRequest("Only shipped orders can be completed.");
            var details = _orderDRepo.GetAll(u => u.OrderHeaderId == orderHeader.Id).ToList();
            foreach (var detail in details)
            {
                var product = _prodRepo.Find(detail.ProductId);
                if (product == null || product.Stock < detail.Qty)
                    return BadRequest("Insufficient stock to complete this order.");
            }
            orderHeader.OrderStatus = WC.StatusCompleted;

            TempData[WC.Success] = "Action completed successfully";
            foreach (var detail in details)
            {
                Product prodTemp = _prodRepo.FirstOrDefault(x => x.Id == detail.ProductId);
                prodTemp.Stock -= detail.Qty;
            }
            _prodRepo.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
