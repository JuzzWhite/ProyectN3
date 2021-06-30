using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models.View_Model
{
    public class LandingPageVM
    {
        public IEnumerable<CatalogueItem> CatalogueItem { get; set; }
        public IEnumerable<Category>Category { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }

    }
}
