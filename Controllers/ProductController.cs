using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using TL_Clothing.Interface;
using TL_Clothing.Models;
using TL_Clothing.ViewModels;

namespace TL_Clothing.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<Customer> _usermanager;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<Customer> usermanager)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _usermanager = usermanager;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();
            return View(products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ProductVm product = new ProductVm()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.CategoryId
                })
            };
            return View(product);
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {

            if (product.Image != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\product");

                using var fileStream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                product.Image.CopyTo(fileStream);

                product.ImageUrl = @"\images\product\" + filename;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x402";
            }
            product.ProductId = Guid.NewGuid().ToString();
            product.CreatedDate=DateTime.Now;   
            _unitOfWork.Product.Add(product);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var product = _unitOfWork.Product.Get(x => x.ProductId == id);
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (product.Image != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\product");

                using var fileStream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                product.Image.CopyTo(fileStream);

                product.ImageUrl = @"\images\product\" + filename;
            }
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var product =_unitOfWork.Product.Get(x=>x.ProductId == id);
            return View(product);
        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {
            var objfromDb = _unitOfWork.Product.Get(x => x.ProductId == product.ProductId);
            if (objfromDb != null)
            {
                if (objfromDb is not null)
                {
                    if (!string.IsNullOrEmpty(objfromDb.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objfromDb.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    _unitOfWork.Product.Remove(objfromDb);
                    _unitOfWork.Save();

                    return RedirectToAction(nameof(Index));

                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]   
        public IActionResult Details(string id)
        {
            var product=_unitOfWork.Product.Get(x=>x.ProductId == id);
            var images =_unitOfWork.ProductImage.GetAll(x=>x.ProductId==id);
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.Customer.Get(x => x.UserName == username);

          var review=_unitOfWork.ProductReview.GetAll(x=>x.ProductId==id);
            foreach(var item in review)
            {
                item.User = _unitOfWork.Customer.Get(c => c.Id == item.UserId);
            }

            ProductDetailsVm vm = new ProductDetailsVm() 
            { 
              Product = product,
              Images = images,
              ProductReview=review,
            };

            if(vm.Product is not null)
            {
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }
    }
}
