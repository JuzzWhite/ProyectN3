using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TecnologyShop.Data;
using TecnologyShop.Models;
using TecnologyShop.Models.View_Model;
using TecnologyShop.Utility;

namespace TecnologyShop.Controllers
{
    [Area("Sales")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            LandingPageVM model = new LandingPageVM()
            {
                CatalogueItem = await _db.CatalogueItem.Include(x => x.Category).Include(x=>x.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(x => x.IsActive == true).ToListAsync()
            };

            return View(model);
        }

        [Authorize]
        //[Authorize(Roles = "Manager,Counter,Sales")]
        public async Task<IActionResult> Details(int id)
        {
            var CatalogueItemFromDb =
              await _db.CatalogueItem.Include(m => m.Category).Include(
                  m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();
            if (CatalogueItemFromDb.Stock.Equals("1"))
            {
                ViewBag.Stock = "On stock";
            }
            if (CatalogueItemFromDb.Stock.Equals("0"))
            {
                ViewBag.Stock = "Out Stock";
            }
            
            string strFecha = CatalogueItemFromDb.Category.RegisterDate.ToString();
            //"23/05/2021 0:00:00"
            //"2021-05-23"
            strFecha = strFecha.Substring(6, 4) + "-" + strFecha.Substring(3, 2) +
                       "-" + strFecha.Substring(0, 2);
            ViewBag.Fecha = strFecha;
            ShoppingCart carObj = new ShoppingCart()
            {
                CatalogueItem = CatalogueItemFromDb,
                CatalogueItemID = CatalogueItemFromDb.Id
            };
            return View(carObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                //obtener todas las notificaciones de autorizacion del usuario actual
                //Name, Role, etc (ClaimsType) desde la app en uso
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                //Una vez obtenido el usuario y sus propiedades, buscamos en la base de datos
                //para verificar la autenticidad y asignar los roles que tiene
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //la propiedad ApplicationUserID se le asignan los valores de claim
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId
                                          == CartObject.ApplicationUserId
                                          && c.CatalogueItemID == CartObject.CatalogueItemID).FirstOrDefaultAsync();
                if (cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(CartObject);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + CartObject.Count;
                }
                await _db.SaveChangesAsync();
                var count = _db.ShoppingCart.Where(c =>
                              c.ApplicationUserId == CartObject.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {
                var CatalogueItemFromDb = await _db.CatalogueItem.Include(m =>
                                     m.Category).Include(m => m.SubCategory).Where(m =>
                                     m.Id == CartObject.CatalogueItemID).FirstOrDefaultAsync();
                if (CatalogueItemFromDb.Stock.Equals("0"))
                {
                    ViewBag.Stock = "out Stock";
                }
                if (CatalogueItemFromDb.Stock.Equals("1"))
                {
                    ViewBag.Stock = "On stock";
                }

                ShoppingCart cartObj = new ShoppingCart()
                {
                    CatalogueItem = CatalogueItemFromDb,
                    CatalogueItemID = CatalogueItemFromDb.Id
                };
                return View(cartObj);
            }
        }

        public IActionResult Privacy()
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
