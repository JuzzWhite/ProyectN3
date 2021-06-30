using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product name")]
        public String Name { get; set; }
        [Required]
        [Display(Name = "Category Id")]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
