using TL_Clothing.Data;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Repositories
{
    public class CategoryRepo : Repository<Category>, ICategory
    {
        public CategoryRepo(SqlDbContext context) : base(context)
        {
        }
    }
}
