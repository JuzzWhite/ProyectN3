using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string CouponType { get; set; }
        public enum ECouponType { percent=0, Colones=500}
        public double Discount { get; set; }
        public double MinimunAmount { get; set; }
        public byte[] picture { get; set; }
        public bool IsActive { get; set; }





    }
}
