using Microsoft.EntityFrameworkCore;
using TL_Clothing.Data;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Repositories
{
    public class OrderRepo : IOrder
    {

        private readonly SqlDbContext _db;

        public OrderRepo(SqlDbContext db)
        {
            _db = db;
        }
        public async Task AddOrderItem(OrderItem item)
        {
            await _db.OrderItems.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task AddOrderItemAsRange(List<OrderItem> orderItems)
        {
            await _db.OrderItems.AddRangeAsync(orderItems);
        }

        public async Task<Order> Create(Order order)
        {
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task DeleteOrder(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }

        public async Task<Order> Details(string email)
        {
            return await _db.Orders
                .Include(i => i.OrderItems)
                .Include(a => a.User)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.User.Email == email);
        }

        public async Task<Order> GetOrderByOrderNumber(string orderNumber)
        {
            return await _db.Orders.Include(o=>o.OrderItems)
              .AsTracking()
             .FirstOrDefaultAsync(a => a.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItems(string id)
        {
            var orderItems = await _db.OrderItems
               .Include(a => a.Product)
               .Where(a => a.OrderId == id).ToListAsync();

            return orderItems;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _db.Orders
               .Include(i => i.OrderItems)
               .AsSplitQuery()
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(string id)
        {
            return await _db.Orders
              .Include(i => i.OrderItems)
              .AsNoTracking().
              Where(a => a.UserId == id)
              .ToListAsync();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateOrder(Order order)
        {
            _db.Orders.Update(order);
           await _db.SaveChangesAsync();
        }
    }
}
