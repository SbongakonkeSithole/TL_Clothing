using TL_Clothing.Data;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Repositories
{
    public class ProductReviewRepo : Repository<ProductReview>, IProductReview
    {
        public ProductReviewRepo(SqlDbContext context) : base(context)
        {
        }
    }
}
