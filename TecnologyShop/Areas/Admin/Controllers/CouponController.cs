using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TecnologyShop.Data;
using TecnologyShop.Models;

namespace TecnologyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        { 
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View( await _db.Coupon.ToListAsync());
        }


        //get- create
        public IActionResult Create()
        {
             return  View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon cupon)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count()>0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                     {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();

                        }
                     }
                    cupon.picture = p1;
                }
                _db.Coupon.Add(cupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();


        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cupon = await _db.Coupon.FirstOrDefaultAsync(x => x.Id == id);

            if (cupon == null)
            {
                return NotFound();
            }

            return View(cupon);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon cupon)
        {
            if(cupon.Id==0)
            {
                return NotFound();
            }

            var cuponFromDb = await _db.Coupon.Where(c => c.Id == cupon.Id).FirstOrDefaultAsync();

            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if(files.Count()>0)
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    cuponFromDb.picture = p1; 
                }
                cuponFromDb.Name = cupon.Name;
                cuponFromDb.Discount = cupon.Discount;
                cuponFromDb.MinimunAmount = cupon.MinimunAmount;
                cuponFromDb.CouponType = cupon.CouponType;
                cuponFromDb.IsActive = cupon.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cupon);


        }


        //DETAILS-get
        public async Task<IActionResult> Details(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var cupon = await _db.Coupon.FirstOrDefaultAsync(x=>x.Id==id);

            if(cupon ==null)
            {
                return NotFound(); 
            }
            return View(cupon);
        }



        //DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cupon = await _db.Coupon.FirstOrDefaultAsync(x => x.Id == id);

            if (cupon == null)
            {
                return NotFound();
            }
            return View(cupon);
        }

        //DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cupon = await _db.Coupon.SingleOrDefaultAsync(x => x.Id == id);
            _db.Remove(cupon);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
