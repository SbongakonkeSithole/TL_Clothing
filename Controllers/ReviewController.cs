using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Customer> _usermanager;


        public ReviewController(IUnitOfWork unitOfWork, UserManager<Customer> usermanager)
        {
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
        }

        public IActionResult Index()
        {
            var reviews = _unitOfWork.ProductReview.GetAll();
            return View(reviews);
        }

        [HttpGet]
        public IActionResult Create(string productId)
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.Customer.Get(x => x.UserName == username);

            ProductReview review = new ProductReview()
            {
                ProductId = productId,
                UserId =user.Id,
                ReviewId=Guid.NewGuid().ToString(),
            };
            return View(review);
        }
        [HttpPost]
        public IActionResult Create(ProductReview review1)
        {
            _unitOfWork.ProductReview.Add(review1);
            _unitOfWork.Save();
            return RedirectToAction("GetUserOrders", "Order");
        }
    }
}
