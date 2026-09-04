using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IProductService _productService;

        private readonly IShoppingCartService _shoppingCartService;
        public CartController(IProductService productService, IShoppingCartService shoppingCartService)
        {
            this._productService = productService;
            this._shoppingCartService = shoppingCartService;
        }
        public async Task<IActionResult> Index()
        {
               return View();
        }
      
    }
}
