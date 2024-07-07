using System.Linq.Expressions;
using TL_Clothing.Models;

namespace TL_Clothing.Interface
{
    public interface ICart
    {
        CartItem GetCartItem(int cartId, string productId);
        void CreateCart(Cart cart);
        void DeleteCart(int cartid);
        void UpdateCartItem(CartItem cartItem);
        Cart GetCart(string id);

        void AddItemToCart(CartItem cartItem);

        void RemoveCartItem(int id);

        int GetCartQuantity(string id);
      
    }
}
