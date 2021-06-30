using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Extensions
{

    //https://stackoverflow.com/questions/27235295/how-to-add-item-to-ienumerable-selectlistitem 
    // <select id = "ddlCategorylist" asp-for="SubCategory.CategoryId" 
    //asp-items="Model.CategoryList.ToSelectListItem(Model.SubCategory.CategoryId)" 
    //class="form-control"></select>
    //TDA Tabla de Datos Abstractos
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem>
                     ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}
