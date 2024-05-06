using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using BulkyWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        public readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            //  List<Product> product = _unitOfWork.product.GetAll().ToList();
            //  ProductVM productvm = product.ToList();
            List<Product> product = _unitOfWork.product.GetAll(includeProperties: "Category").ToList();
                //.ToList();
            return View(product);
        }

        #region Ajax API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> product = _unitOfWork.product.GetAll(includeProperties: "Category").ToList();
            //.ToList();
            return Json(new { data = product });
        }

        [HttpDelete]
        public IActionResult DeleteAPI(int id)
        {
            var productToBeDeleted = _unitOfWork.product.Get(u => u.Id == id);

            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deteting Product" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new {success=true,message="Successfully Deleted Product"});
        }

        #endregion Ajax API CALL

        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> Categorylist = _unitOfWork.category
            //    .GetAll().ToList().Select(u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()
            //    });
            // ViewBag.Categorylist = Categorylist;
            ProductVM productvm = new()
            {
                CategoryList = _unitOfWork.category
                    .GetAll().ToList().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {              
                return View(productvm);
            }
            else
            {
                productvm.Product = _unitOfWork.product.Get(u => u.Id == id);
                return View (productvm);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM objCreate , IFormFile? file)
        {
            

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file !=null)
                {
                    string filename = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"image\product");
              
                    if(!string.IsNullOrEmpty(objCreate.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, objCreate.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);    
                        } 
                    }

                    using(var filestream = new FileStream(Path.Combine(productPath,filename), FileMode.Create ))
                    {
                        file.CopyTo(filestream);
                    }

                    objCreate.Product.ImageUrl = @"\image\product\" + filename;
                }
                else
                {
                    objCreate.Product.ImageUrl = objCreate.Product.ImageUrl;
                }
                if (objCreate.Product.Id == 0 || objCreate.Product.Id == null)
                {
                    _unitOfWork.product.Add(objCreate.Product);
                }
                else
                {
                    _unitOfWork.product.update(objCreate.Product);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Product Created Successfully !";
            }
            else
            {
                objCreate.CategoryList = _unitOfWork.category
                    .GetAll().ToList().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                TempData["error"] = "Product  Creation Failed!";
                return View(objCreate);
            }
            //return View(Index());
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (id != null)
            {
                var product = _unitOfWork.product.Get(u => u.Id == id);
                if (product != null)
                {
                    ProductVM productVM = new()
                    {
                        CategoryList = _unitOfWork.category.GetAll().ToList().Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString()
                        }),
                        Product = _unitOfWork.product.Get(u => u.Id == id)
                    };
                    return View(productVM);
                }
                else
                {
                    TempData["error"] = "Error while selecting Product!";
                    return NotFound();
                }

            }
            else
            {
                TempData["error"] = "Error while selecting Product!";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(ProductVM objEdit)
        {
            if (objEdit != null)
            {
                _unitOfWork.product.update(objEdit.Product);
                _unitOfWork.Save();

                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Error while Deleting Product!";
                return NotFound();
            }
        }

        public IActionResult Delete(int id)
        {
            if (id != null)
            {
                var product = _unitOfWork.product.Get(u => u.Id == id);
                if (product != null)
                {
                    ProductVM productvm = new()
                    {
                        CategoryList = _unitOfWork.category.GetAll().ToList().Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString()
                        }),
                        Product = _unitOfWork.product.Get(u => u.Id == id)
                    };

                    return View(productvm);
                }
                else
                {
                    TempData["error"] = "Error while selecting Product!";
                    return NotFound();
                }

            }
            else
            {
                TempData["error"] = "Error while selecting Product!";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public IActionResult Delete(ProductVM objDelete)
        {
            try
            {
                if (objDelete != null)
                {
                    _unitOfWork.product.Remove(objDelete.Product);
                    _unitOfWork.Save();

                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["error"] = "Error while Deleting Product!";
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message.ToString();
                return NotFound();
            }
        }
    }
}
