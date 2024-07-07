using TL_Clothing.Data;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Repositories
{
    public class ProductRepo : Repository<Product>, IProduct
    {
        public ProductRepo(SqlDbContext context) : base(context)
        {
        }
    }
}
