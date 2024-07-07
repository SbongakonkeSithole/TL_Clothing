using TL_Clothing.Data;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Repositories
{
    public class CustomerRepo : Repository<Customer>, ICustomer
    {
        private readonly SqlDbContext _context;
        public CustomerRepo(SqlDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
