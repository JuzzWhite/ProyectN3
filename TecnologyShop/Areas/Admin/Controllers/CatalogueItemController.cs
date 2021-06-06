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

        /*   [HttpPost, ActionName("Create")]
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
               }




           }
        */
    }
}
