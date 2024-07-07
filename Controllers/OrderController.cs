using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayStack.Net;
using TL_Clothing.Interface;
using TL_Clothing.Models;
using TL_Clothing.ViewModels;

namespace TL_Clothing.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ICart _cart;
        private readonly IOrder _order;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Customer> _usermanager;
        private readonly string key;
        private readonly IPayStackApi _payStackApi;
        public OrderController(ICart cart, IUnitOfWork unitOfWork, UserManager<Customer> usermanager, IConfiguration configuration, IOrder order)
        {
            _cart = cart;
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
            _configuration = configuration;
            key = _configuration["PayStackSecret"] ?? throw new NullReferenceException("API NOT FOUND");
            _payStackApi = new PayStackApi(key);
            _order = order;
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
        public async Task<IActionResult> Verify(string reference, string cref, string uid,string? street,string? city, string? state, string? code,string? country,bool deliver)
        {
            TransactionVerifyResponse response = _payStackApi.Transactions.Verify(reference);

            if (response.Data.Status == "success")
            {
                var user =  FindUser();
                Order order = new Order();
                order.UserId = user.Id;
                order.OrderStatus = "ORDER RECIVED";
                order.OrderNumber = GenerateOrderNumber();
                order.Deliver = deliver;
                if (deliver == true)
                {
                    order.Street = street;
                    order.City = city;
                    order.State = state;
                    order.ZipCode = code;
                    order.Country = country;
                    
                }
             

                var cart =  _cart.GetCart(user.Id);
                foreach (var item in cart.CartItem)
                {
                    item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);
                    item.Total = item.Product.Price * item.ProductQuantity;
                    item.Cart.Subtotal += item.Total;
                }
                cart.Total = cart.Subtotal + 100;
                order.TotalPrice = cart.Total;
                order.DateCreated = DateTime.Now;
                order.DateUpdated = DateTime.Now;
                order.Id=reference;

               await   _order.Create(order);

                order = await _order.GetOrderByOrderNumber(order.OrderNumber);

                foreach (var product in cart.CartItem)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.ProductId = product.ProductId;
                    orderItem.OrderId = order.Id;
                    orderItem.ProductQuantity = product.ProductQuantity;
                    orderItem.Total = product.Total;
                    await _order.AddOrderItem(orderItem);

                }

                cart =  _cart.GetCart(user.Id);

                foreach (var item in cart.CartItem)
                {
                    var product =_unitOfWork.Product.Get(x=>x.ProductId == item.ProductId);
                    product.Quantity -= item.ProductQuantity;
                    _unitOfWork.Product.Update(product);
                    _cart.RemoveCartItem(item.Id);
                   
                }
                _cart.DeleteCart(cart.Id);
                return RedirectToAction("GetUserOrder", "Order", new { orderNumber = order.OrderNumber });

            }
            else
            {
                var cart = _cart.GetCart(uid);
                foreach (var item in cart.CartItem)
                {
                    item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);
                    item.Total = item.Product.Price * item.ProductQuantity;
                    item.Cart.Subtotal += item.Total;
                }
                cart.Total = cart.Subtotal + 100;
                //erro message
                return View("Cart", cart);
            }
        }
        public async Task<IActionResult> GetUserOrder(string orderNumber)
        {
            var order = await _order.GetOrderByOrderNumber(orderNumber);
            foreach (var item in order.OrderItems)
            {
                item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);
             
            }
            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateOrder(string orderNumber)
        {
            var order = await _order.GetOrderByOrderNumber(orderNumber);
            foreach (var item in order.OrderItems)
            {
                item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);

            }
            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            var objFromDb = await _order.GetOrderByOrderNumber(order.OrderNumber);
            foreach (var item in objFromDb.OrderItems)
            {
                item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);

            }
            objFromDb.OrderStatus = order.OrderStatus;
            objFromDb.DateUpdated = DateTime.Now;
            await _order.UpdateOrder(objFromDb);
            return RedirectToAction("GetAllOrder");
        }
        public async Task<IActionResult> GetAllOrder()
        {
            var orders = await _order.GetOrders();
            foreach(var item in orders)
            {
                item.User = _unitOfWork.Customer.Get(m => m.Id == item.UserId);
            }
             return View(orders);
        }
        public async Task<IActionResult> GetUserOrders()
        {
            var user = FindUser();
            if(user is not null)
            {
                var orders = await _order.GetOrdersByUserId(user.Id);
                foreach(var item in orders)
                {
                    foreach (var item2 in item.OrderItems)
                    {
                        item2.Product=_unitOfWork.Product.Get(c=>c.ProductId == item2.ProductId);
                    }
                }
                return View("CurrentUserOrders", orders);
            }
            else
            {
                //TODO FIX THE ERROR MESSAGE
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public IActionResult TrackOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> TrackOrder(TrackVm track)
        {
            var order = await _order.GetOrderByOrderNumber(track.OrderNumber);
            if(order is not null)
            {
                foreach (var item in order.OrderItems)
                {
                    item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);

                }
                return View("GetUserOrder", order);
            }
            else
            {
                return View();
            }
         
            
           
        }
        public async Task<IActionResult> OrderDetails(string orderNumber)
        {
            var order = await _order.GetOrderByOrderNumber(orderNumber);
            foreach (var item in order.OrderItems)
            {
                item.Product = _unitOfWork.Product.Get(u => u.ProductId == item.ProductId);

            }
            order.User = _unitOfWork.Customer.Get(o => o.Id == order.UserId);
            return View(order);
        }
        public string GenerateOrderNumber()
        {
            return "#" + new Random().Next(1000) + "-" + new Random().Next(100);
        }
    }
}
