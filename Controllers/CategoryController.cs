using Microsoft.AspNetCore.Mvc;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var cat = _unitOfWork.Category.GetAll();
            return View(cat);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            obj.CategoryId = Guid.NewGuid().ToString();
            if (obj.Image != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\category");

                using var fileStream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                obj.Image.CopyTo(fileStream);

                obj.CategoryImageUrl = @"\images\category\" + filename;
            }
            else
            {
                obj.CategoryImageUrl = "https://placehold.co/600x402";
            }
            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var objfromDb = _unitOfWork.Category.Get(x => x.CategoryId == id);
            if (objfromDb != null)
            {
                if (objfromDb is not null)
                {
                    if (!string.IsNullOrEmpty(objfromDb.CategoryImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objfromDb.CategoryImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    _unitOfWork.Category.Remove(objfromDb);
                    _unitOfWork.Save();

                    return RedirectToAction(nameof(Index));

                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var objFromDb = _unitOfWork.Category.Get(x => x.CategoryId == id);
            return View(objFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Image != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(category.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\uploads");

                if (!string.IsNullOrEmpty(category.CategoryImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, category.CategoryImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using var fileStream = new FileStream(Path.Combine(imagePath, filename), FileMode.Create);
                category.Image.CopyTo(fileStream);

                category.CategoryImageUrl = @"\images\uploads\" + filename;
            }

            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
