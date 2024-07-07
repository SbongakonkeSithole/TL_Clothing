using TL_Clothing.Data;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Repositories
{
    public class ProductImageRepo : Repository<ProductImage>, IProductImage
    {
        public ProductImageRepo(SqlDbContext context) : base(context)
        {
        }
    }
}
