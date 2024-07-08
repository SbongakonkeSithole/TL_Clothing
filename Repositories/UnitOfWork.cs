using TL_Clothing.Data;
using TL_Clothing.Interface;

namespace TL_Clothing.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly SqlDbContext _db;

        public ICustomer Customer { get; private set; }
        public ICategory Category { get; private set; }
        public IProduct Product { get; private set; }
        public IProductImage ProductImage { get; private set; }
        public IProductReview ProductReview { get; private set; }
        public UnitOfWork(SqlDbContext db)
        {
            _db = db;
            Customer = new CustomerRepo(_db);
            Product = new ProductRepo(_db);
            ProductImage = new ProductImageRepo(_db);
            Category = new CategoryRepo(_db);
            ProductReview = new ProductReviewRepo(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
