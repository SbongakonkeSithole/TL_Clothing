using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TL_Clothing.Data;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Repositories
{
    public class CartRepo:ICart
    {
        private readonly SqlDbContext _db;

        public CartRepo(SqlDbContext db)
        {
            _db = db;
        }
        public void DeleteCart(int cartid)
        {
            var cart =_db.Carts.FirstOrDefault(x=>x.Id == cartid);
            _db.Carts.Remove(cart);
            _db.SaveChanges();
        }

        public void AddItemToCart(CartItem cartItem)
        {
            _db.CartItems.Add(cartItem);
            _db.SaveChanges();
        }

        public void CreateCart(Cart cart)
        {
            _db.Carts.Add(cart);
            _db.SaveChanges();
        }

        public Cart GetCart(string id)
        {
            var cart = _db.Carts
                 .AsSplitQuery()
                 .Include(z => z.CartItem)
                 .Where(a => a.UserId == id)
                 .FirstOrDefault();

            return cart;
        }

        public int GetCartQuantity(string id)
        {
            int quantity = 0;
            var cart = _db.Carts
              .Include(z => z.CartItem)
              .Where(a => a.UserId == id)
              .FirstOrDefault();


            if (cart == null)
            {
                return quantity;
            }

            quantity = cart.CartItem.Count();


            return quantity;
        }

        public void RemoveCartItem(int id)
        {
            var cartItem = _db.CartItems.Find(id);


            if (cartItem != null)
            {
                _db.CartItems.Remove(cartItem);
                _db.SaveChanges();
            }
        }
          public CartItem GetCartItem(int cartId, string productId)
          {
            
             return _db.CartItems
                           .FirstOrDefault(ci => ci.CartId == cartId && ci.ProductId == productId);
          }
        public void UpdateCartItem(CartItem cartItem)
        {
            _db.CartItems.Update(cartItem);
            _db.SaveChanges();
        }

    }
}
