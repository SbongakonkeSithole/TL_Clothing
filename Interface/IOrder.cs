using TL_Clothing.Models;

namespace TL_Clothing.Interface
{
    public interface IOrder
    {
        Task<IEnumerable<Order>> GetOrders();

        Task<Order> GetOrderByOrderNumber(string orderNumber);

        Task<IEnumerable<Order>> GetOrdersByUserId(string id);

        Task AddOrderItem(OrderItem item);
        Task AddOrderItemAsRange(List<OrderItem> orderItems);

        Task<Order> Create(Order order);

        Task<Order> Details(string email);

        Task<IEnumerable<OrderItem>> GetOrderItems(string id);

        Task DeleteOrder(int id);
        Task UpdateOrder(Order order);

        Task Save();
    }
}
