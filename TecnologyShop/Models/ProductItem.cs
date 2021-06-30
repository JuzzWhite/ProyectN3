using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models
{
    public class ProductItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Display(Name = "Category Id")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,####}")]
        [Range(1, int.MaxValue, ErrorMessage = "Price should" +
            " be greater than ₡{1}")]
        public double Price { get; set; }


    }
}
