﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
   public virtual ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "Order Total original")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OrderTotalOriginal { get; set; }
        [Required]
        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OrderTotal { get; set; }
        [Required]
        [Display(Name = "Pickup Time")]
        public DateTime  PickUpTime { get; set; }
        [Required]
        [NotMapped]
        [Display(Name = "Pickup Date")]
        public DateTime PickUpDate { get; set; }
        [Display(Name = "Coupon Code")]
        public string CouponCode { get; set; } 
        public double CouponCodeDiscount { get; set; } 
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Pickup Name")]
        public string PickupName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string TransactionId { get; set; }
    }
}
