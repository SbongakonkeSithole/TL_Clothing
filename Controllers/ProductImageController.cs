using Microsoft.AspNetCore.Mvc;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductImageController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var Image = _unitOfWork.ProductImage.GetAll();
            foreach(var t in Image)
            {
                t.Product=_unitOfWork.Product.Get(o=>o.ProductId==t.ProductId);
            }
            return View(Image);
        }
        [HttpGet]
        public IActionResult Create(string productId)
        {
            ProductImage product = new ProductImage()
            {
                ProductId = productId,
                ImageId = Guid.NewGuid().ToString(),
            };
            return View(product);
        }
        [HttpPost]
        public IActionResult Create(ProductImage product)
        {
            if (product.Image != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\producturl");

                using var fileStream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                product.Image.CopyTo(fileStream);

                product.ImageUrl = @"\images\producturl\" + filename;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x402";
            }
            _unitOfWork.ProductImage.Add(product);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var image = _unitOfWork.ProductImage.Get(x=>x.ImageId==id);
            return View(image);
        }
        [HttpPost]
        public IActionResult Delete(ProductImage product)
        {
            var image =_unitOfWork.ProductImage.Get(c=>c.ImageId==product.ImageId);
            if (image is not null)
            {
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(image);
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View(image);
            }
        }
    }
}
