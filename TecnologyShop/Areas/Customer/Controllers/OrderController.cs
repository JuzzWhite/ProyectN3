using TecnologyShop.Data;
using TecnologyShop.Models;
using TecnologyShop.Models.View_Model;
using TecnologyShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore; // recien añadido
using Rotativa.AspNetCore; // recien añadido

namespace TecnologyShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;
        readonly IGeneratePdf _generatePdf; // añadido recientemente

        //public OrderListViewModel OList { get; set; } //añadido recientemente

        public OrderController(ApplicationDbContext db, IGeneratePdf generatePdf /*añadido recientemente*/)
        {
            _db = db;
            _generatePdf = generatePdf; // añadido recientemente
        }
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(o => 
                            o.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()
            };
            return View(orderDetailsViewModel);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetOrderStatus(int Id)
        {
            return PartialView("_OrderStatus", _db.OrderHeader.Where(m => m.Id == Id).FirstOrDefault().Status);

        }

        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            OrderListViewModel OrderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };
            //List<OrderDetailsViewModel> orderList = new List<OrderDetailsViewModel>();
            List<OrderHeader> OrderHeaderList =
                await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.UserId == 
                     claim.Value).ToListAsync();
            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                OrderListVM.Orders.Add(individual);
            }
            var count = OrderListVM.Orders.Count;
            OrderListVM.Orders = OrderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id).ToList();
            return View(OrderListVM);
        }


        [Authorize(Roles = SD.ManagerUser + "," + SD.SalesAgent)] 
        public async Task<IActionResult> ManageOrder()
        {
            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();
            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Where(o => o.Status 
                        == SD.StatusSubmitted || o.Status 
                        == SD.StatusInProcess).OrderByDescending(u => 
                        u.PickUpTime).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderDetailsVM.Add(individual);
            }
            return View(orderDetailsVM.OrderBy(o => o.OrderHeader.PickUpTime).ToList());
        }


        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            //int x = Id;
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.ApplicationUser =
                await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == 
                                       orderDetailsViewModel.OrderHeader.UserId);
            return PartialView("_IndividualOrderDetails", orderDetailsViewModel);
        }
        [Authorize(Roles = SD.SalesAgent + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }


        [Authorize(Roles = SD.SalesAgent + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusReady;
            await _db.SaveChangesAsync();

            //Email logic to notify user that order is ready for pickup
            //await IEmailSender.SendEmailAsync(_db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Spice - Order Ready for Pickup " + orderHeader.Id.ToString(), "Order is ready for pickup.");


            return RedirectToAction("ManageOrder", "Order");
        }


        [Authorize(Roles = SD.SalesAgent + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            //await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Spice - Order Cancelled " + orderHeader.Id.ToString(), "Order has been cancelled successfully.");

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize]
        public async Task<IActionResult> OrderPickup(string searchEmail = null, string searchPhone = null, string searchName = null)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }

            List<OrderHeader> OrderHeaderList = new List<OrderHeader>();
            if (searchName != null || searchEmail != null || searchPhone != null)
            {
                var user = new ApplicationUser();

                if (searchName != null)
                {
                    OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                .Where(u => u.PickupName.ToLower().Contains(searchName.ToLower()))
                                                .OrderByDescending(o => o.OrderDate).ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _db.ApplicationUser.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                        OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                    .Where(o => o.UserId == user.Id)
                                                    .OrderByDescending(o => o.OrderDate).ToListAsync();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                        .Where(u => u.PhoneNumber.Contains(searchPhone))
                                                        .OrderByDescending(o => o.OrderDate).ToListAsync();
                        }
                    }
                }
            }
            else
            {
                OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.Status == SD.StatusReady).ToListAsync();
            }

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id).ToList();

            //orderListVM.PagingInfo = new PagingInfo
            //{
            //    CurrentPage = productPage,
            //    ItemsPerPage = PageSize,
            //    TotalItem = count,
            //    urlParam = param.ToString()
            //};

            return View(orderListVM);
        }

        [Authorize(Roles = SD.FrontDeskUser + "," + SD.ManagerUser)]
        [HttpPost]
        [ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int orderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            //await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Spice - Order Completed " + orderHeader.Id.ToString(), "Order has been completed successfully.");

            return RedirectToAction("OrderPickup", "Order");
        }


        // añadido recientemente, metodo para imprimir pdf

        public async Task<IActionResult> GetPDF(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(o =>
                            o.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()
            };
            //return View(orderDetailsViewModel);

            //if (OList.CatalogueItem == null)
            //{
            //    return NotFound();
            //}
            return new ViewAsPdf("ReportProduct", orderDetailsViewModel)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait, //para modificar la pagina
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

        }


    }
}
