using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models.View_Model
{
    public class OrderListViewModel
    {
        public IList<OrderDetailsViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}


