using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecnologyShop.Data;
using TecnologyShop.Models.View_Model;
using System.IO;
using Microsoft.EntityFrameworkCore;
using TecnologyShop.Utility;
using TecnologyShop.Models;

namespace TecnologyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CatalogueItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty]
        public CatalogueViewModel catalogueVM { get; set; }

        public CatalogueItemController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            catalogueVM = new CatalogueViewModel()
            {
                Category = _db.Category,
                CatalogueItem = new Models.CatalogueItem()
            };
        }

        public async Task<IActionResult> Index()
        {
            var Catalogue = await _db.CatalogueItem.Include(x => x.Category).Include(x => x.SubCategory).ToArrayAsync();

            return View(Catalogue);
        }

        //get-create
        public async Task<IActionResult> Create()
        {
            return View(catalogueVM);
        }



          [HttpPost, ActionName("Create")]
           [ValidateAntiForgeryToken]
           public async Task<IActionResult> CreatePost()
           {
               catalogueVM.CatalogueItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

               if (!ModelState.IsValid)
               {
                   return View(catalogueVM);
               }

               _db.CatalogueItem.Add(catalogueVM.CatalogueItem);
               await _db.SaveChangesAsync();


               //work on the image saving section
               string WebRootPath = _hostEnvironment.WebRootPath;
               var files = HttpContext.Request.Form.Files;

               var prodItemFormDB = await _db.CatalogueItem.FindAsync(catalogueVM.CatalogueItem.Id);

               if (files.Count() > 0)
               {
                   //files has been uploaded
                   var uploads = Path.Combine(WebRootPath, "Images");
                   var extension = Path.GetExtension(files[0].FileName);
                   using (var filestream = new FileStream(Path.Combine(uploads, catalogueVM.CatalogueItem.Id + extension), FileMode.Create))
                   {
                       files[0].CopyTo(filestream);
                   }
                prodItemFormDB.Image = @"\images\" + catalogueVM.CatalogueItem.Id + extension;
               }

                else
                {
                 //NO file was uploaded, so use default
                 var uploads = Path.Combine(WebRootPath, @"\image" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, WebRootPath + @"\images" +
                    catalogueVM.CatalogueItem.Id + ".png");
                prodItemFormDB.Image = @"\image" + catalogueVM.CatalogueItem.Id + ".png";
                }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


           }
        //Get-Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            catalogueVM.CatalogueItem = await _db.CatalogueItem.Include(x => x.Category).Include(x => x.SubCategory).SingleOrDefaultAsync(x => x.Id == id);
            catalogueVM.SubCategory = await _db.SubCategory.Where(x => x.CategoryId == catalogueVM.CatalogueItem.Category.Id).ToListAsync();

            if (catalogueVM.CatalogueItem == null)
            {
                return NotFound();
            }

            return View(catalogueVM);


        }




        //Solo imagenes .jpg
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            catalogueVM.CatalogueItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                catalogueVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == catalogueVM.CatalogueItem.CategoryId).ToListAsync();
                return View(catalogueVM);
            }

            //Work on the image saving section
            string webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await _db.CatalogueItem.FindAsync( catalogueVM.CatalogueItem.Id);


            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file 

                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, catalogueVM.CatalogueItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + catalogueVM.CatalogueItem.Id + extension_new;
            }

            menuItemFromDb.Name = catalogueVM.CatalogueItem.Name;
            menuItemFromDb.Description = catalogueVM.CatalogueItem.Description;
            menuItemFromDb.Price = catalogueVM.CatalogueItem.Price;
            menuItemFromDb.Stock = catalogueVM.CatalogueItem.Stock;
            menuItemFromDb.CategoryId = catalogueVM.CatalogueItem.CategoryId;
            menuItemFromDb.SubCategoryId = catalogueVM.CatalogueItem.SubCategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            catalogueVM.CatalogueItem = await _db.CatalogueItem.Include(m =>
                   m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m =>
                   m.Id == id);

            if (catalogueVM.CatalogueItem == null)
            {
                return NotFound();
            }

            return View(catalogueVM);
        }



        //Get-Delete
        public async Task<IActionResult>Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            catalogueVM.CatalogueItem = await _db.CatalogueItem.Include(m =>
                m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (catalogueVM.CatalogueItem == null)
            {
                return NotFound();
            }

            return View(catalogueVM);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            CatalogueItem catalogueItem = await _db.CatalogueItem.FindAsync(id);

            if (catalogueItem!=null)
            {
                var imagePath = Path.Combine(webRootPath, catalogueItem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.CatalogueItem.Remove(catalogueItem);
                await _db.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }




    }
}
