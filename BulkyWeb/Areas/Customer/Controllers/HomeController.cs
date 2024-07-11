
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> product = _unitOfWork.product.GetAll(includeProperties: "Category").ToList();    
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Details(int productId)
         {
            //var product = _unitOfWork.product.Get(u=>u.Id==productId, includeProperties:"Category");
            //return View(product);

            ShoppingCart Cart = new()
            {
                Product = _unitOfWork.product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };

            return View(Cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCartobj)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                shoppingCartobj.ApplicationUserId = UserId;

                ShoppingCart cartFromDBobj = _unitOfWork.shoppingCart.Get(x => x.ApplicationUserId == shoppingCartobj.ApplicationUserId &&
                x.ProductId == shoppingCartobj.ProductId);

                if(cartFromDBobj != null)
                {   //product already added by user
                    cartFromDBobj.Count += shoppingCartobj.Count;
                    _unitOfWork.shoppingCart.Update(cartFromDBobj);
                }
                else
                {
                    _unitOfWork.shoppingCart.Add(shoppingCartobj);
                }
                //_unitOfWork.shoppingCart.Add(shoppingCart);              
                _unitOfWork.Save();
                TempData["Success"] = "Product Sucessfully Added To Cart!";
            }

            return RedirectToAction(nameof(Index));

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}