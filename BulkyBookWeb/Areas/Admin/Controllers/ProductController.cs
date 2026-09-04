using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utiltiy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            ProductVM productVM = new ProductVM
            {
                Product = new Product(),
                CategoryList = categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                // Create
                return View(productVM);

            }
            else
            {
                // Update
                productVM.Product = await _productService.GetProductByIdAsync(id.Value);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Upsert")]
        public async Task<IActionResult> UpsertPOST(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine("images", "products");
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    // Save the new image file
                    using (var fileStreams = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = Path.Combine(@"\" + productPath, fileName).Replace("\\", "/");
                }

                if (productVM.Product.Id == null || productVM.Product.Id == 0)
                {
                    // Create
                    await _productService.CreateProductAsync(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    // Update
                    await _productService.UpdateProductAsync(productVM.Product);
                    if (productVM.Product == null)
                    {
                        return NotFound();
                    }
                    TempData["success"] = "Product updated successfully";
                }
                return RedirectToAction("Index");
            }
            else
            {
                var categories = await _categoryService.GetAllCategoriesAsync();

                productVM = new()
                {
                    CategoryList = categories.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
                };
                return View(productVM);
            }
        }


        #region API CALLS
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync(true);
            return Json(new { data = products });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { Success = false, message = "Invalid ID" });
            }

            var productToBeDeleted = await _productService.GetProductByIdAsync(id.Value);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //delete product image if that exists
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\' , '/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);       
                }
            }
            await _productService.DeleteProductAsync(id.Value);
            return Json(new { success = true, message = "Delete Successfully" });
        }
        #endregion
    }
}