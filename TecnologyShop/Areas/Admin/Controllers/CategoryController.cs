using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecnologyShop.Data;
using TecnologyShop.Models;

namespace TecnologyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
         private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db; 
        }
        //GET Action Method
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            String strFecha = DateTime.Now.ToString();
            //"23/05/2021 0:00:00"
            //"2021-05-23"
            strFecha = strFecha.Substring(6, 4) + "-" + strFecha.Substring(3, 2) +
                       "-" + strFecha.Substring(0, 2);
            ViewBag.Fecha = strFecha;
            return View();
        }
        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //if valid
                _db.Category.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        //GET - EDIT
        public async Task<IActionResult> Edit(int? id){
            if (id == null) {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null){
                return NotFound();
            }
            String strFecha = category.RegisterDate.ToString();
            //"23/05/2021 0:00:00"
            //"2021-05-23"
            strFecha = strFecha.Substring(6, 4) + "-" + strFecha.Substring(3, 2) +
                       "-" + strFecha.Substring(0, 2);
            ViewBag.Fecha = strFecha;
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _db.Category.FindAsync(id);

            if (category == null)
            {
                return View();
            }
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
