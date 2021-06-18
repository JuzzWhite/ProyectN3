using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models.View_Model
{
    public class CatalogueViewModel
    {
        public CatalogueItem CatalogueItem { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }


    }
}
