using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Register Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString =
            "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RegisterDate { get; set; }

    }
}
