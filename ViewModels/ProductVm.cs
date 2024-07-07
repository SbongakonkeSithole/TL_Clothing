using Microsoft.AspNetCore.Mvc.Rendering;
using TL_Clothing.Models;

namespace TL_Clothing.ViewModels
{
    public class ProductVm
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
