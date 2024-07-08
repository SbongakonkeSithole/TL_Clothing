using Microsoft.EntityFrameworkCore.Query.Internal;

namespace TL_Clothing.Interface
{
    public interface IUnitOfWork
    {
        ICustomer Customer { get; }
        ICategory Category { get; }
        IProduct Product { get; }
        IProductImage ProductImage { get; }
        IProductReview ProductReview { get; }
        void Save();
    }
}
