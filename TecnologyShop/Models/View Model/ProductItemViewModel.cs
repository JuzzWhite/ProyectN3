using TecnologyShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models.View_Model
{
    public class ProductItemViewModel
    {
        public ProductItem ProductItem { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Product> Product { get; set; }

    }
}
