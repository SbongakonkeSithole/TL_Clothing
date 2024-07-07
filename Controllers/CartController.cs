using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayStack.Net;
using TL_Clothing.Interface;
using TL_Clothing.Models;
using TL_Clothing.ViewModels;

namespace TL_Clothing.Controllers
{
    public class CartController : Controller
    {
        private readonly ICart _cart;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Customer> _usermanager;
        private readonly string key;
        private readonly IPayStackApi _payStackApi;
        private readonly IConfiguration _configuration;

        public CartController(ICart cart, IUnitOfWork unitOfWork, UserManager<Customer> usermanager, IConfiguration configuration)
        {


            _cart = cart;
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
            _configuration = configuration;
            key = _configuration["PayStackSecret"] ?? throw new NullReferenceException("API NOT FOUND");
            _payStackApi = new PayStackApi(key);
        }


        private Customer FindUser()
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.Customer.Get(x => x.UserName == username);

            if (user is not null)
            {
                ViewBag.User = user;
                ViewBag.CartCount = _cart.GetCartQuantity(user.Id);
                return user;
            }

            return null;
        }
        public IActionResult RemoveProductFromCart(string id)
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.Customer.Get(x => x.UserName == username);
            var cart = _cart.GetCart(user.Id);

            foreach (var item in cart.CartItem)
            {
                item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);
            }
            var cartItem = cart.CartItem.Where(u=>u.ProductId == id).FirstOrDefault();
            if (cartItem!.ProductQuantity>1)
            {
                cartItem.ProductQuantity -= 1;
                _cart.UpdateCartItem(cartItem);
            }
            else
            {
                _cart.RemoveCartItem(cartItem.Id);
            }
            return RedirectToAction(nameof(Cart));
        }
        [Authorize]
        public IActionResult AddProductToCart(string id)
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.Customer.Get(x => x.UserName == username);

            CartItem cartItem = new CartItem();

            var cart = _cart.GetCart(user.Id);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id
                };

                _cart.CreateCart(cart);
                cart = _cart.GetCart(user.Id);
            }

            var product =_unitOfWork.Product.Get(x=>x.ProductId == id);
          
         
            if (product.Quantity > 0)
            {
                cartItem.ProductId = id;
                cartItem.CartId = cart.Id;


                AddOrUpdateCartItem(cartItem.ProductId, cartItem.CartId);


                return RedirectToAction(nameof(Cart));
            }
            else
            {
                //show notification that the product is out of stock
                return RedirectToAction(nameof(Cart));
            }
           

        }
        public IActionResult RemoveItem(int id)
        {
            _cart.RemoveCartItem(id);
            return RedirectToAction(nameof(Cart));
        }
        [HttpGet]
        [Authorize]
        public IActionResult Cart()
        {
            var user = FindUser();

            if (user != null)
            {
                var cart = _cart.GetCart(user.Id);
                if(cart != null)
                {
                    foreach (var item in cart.CartItem)
                    {
                        item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);
                        item.Total = item.Product.Price * item.ProductQuantity;
                        item.Cart.Subtotal += item.Total;
                    }
                    cart.Total = cart.Subtotal + 100;

                    return View("Cart", cart);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                
            }

            return RedirectToAction("LogOut", "Account");
        }
        public IActionResult Checkout(string UserId)
        {
            var user = FindUser();
            var cart =_cart.GetCart(UserId);
            foreach (var item in cart.CartItem)
            {
                item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);
                item.Total = item.Product.Price * item.ProductQuantity;
                item.Cart.Subtotal += item.Total;
            }
            cart.Total = cart.Subtotal + 100;
            
            OrderVm orderVm = new OrderVm()
            {
                Cart = cart,
            };
            return View(orderVm);
        }
      
        public IActionResult Payment(OrderVm orderVm)
        {
            var user = FindUser();
            var cart = _cart.GetCart(orderVm.Cart.UserId);
            TransactionInitializeRequest request = new()
            {
                AmountInKobo = orderVm.Cart.Total * 100,
                Email = user.Email,
                Reference = Guid.NewGuid().ToString(),
                Currency = "ZAR",
                CallbackUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Order/Verify?cref={cart.Id}&uid={user.Id}&street={orderVm.Street}&city={orderVm.City}&state={orderVm.State}&code={orderVm.ZipCode}&country={orderVm.Country}&deliver={orderVm.Deliver}"
            };

            var response = _payStackApi.Transactions.Initialize(request);
            if (response.Status == true)
            {
                return Redirect(response.Data.AuthorizationUrl);
            }
            else
            {
                foreach (var item in cart.CartItem)
                {
                    item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);
                    item.Total = item.Product.Price * item.ProductQuantity;
                    item.Cart.Subtotal += item.Total;
                }
                cart.Total = cart.Subtotal + 100;
                //erro message
                return View("Cart",cart);
            }
           
        }
        
        public void AddOrUpdateCartItem(string productId, int cartId)
        {
            var cartItem = _cart.GetCartItem(cartId, productId);

            if (cartItem != null)
            {
                // If the item exists, increment the quantity by 1
                cartItem.ProductQuantity += 1;
                _cart.UpdateCartItem(cartItem);
            }
            else
            {
                // If the item does not exist, create a new cart item
                cartItem = new CartItem
                {
                    ProductId = productId,
                    CartId = cartId,
                    ProductQuantity = 1
                };
                _cart.AddItemToCart(cartItem);
            }
        }

    }
}
