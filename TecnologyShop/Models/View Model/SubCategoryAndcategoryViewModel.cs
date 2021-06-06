using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models.View_Model
{
    public class SubCategoryAndcategoryViewModel
    {
        //para generar lista de categorias para dropdownlist
        public IEnumerable<Category> CategoryList { get; set; }

        //crea objeto de tipo subcategoria
        public SubCategory SubCategory { get; set; }

        //genera lista de subcategory
        public List<String> subCategoryList { get; set; }

        //para desplegar un mensaje de estado de la accion a realizar
        public string StatusMessage { get; set; }

    }
}
