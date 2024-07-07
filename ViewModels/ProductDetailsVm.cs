using TL_Clothing.Models;

namespace TL_Clothing.ViewModels
{
    public class ProductDetailsVm
    {
        public Product Product { get; set; }
        public IEnumerable<ProductImage> Images { get; set; }
    }
}
