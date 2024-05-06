using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        //private readonly ICategoryRepository _category;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //List<Category> objcategory = _db.Categories.ToList();
            List<Category> objcategory = _unitOfWork.category.GetAll().ToList();
            return View(objcategory);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DispalyOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Name and Display Order must not be same !");

            }
            if (obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("Name", "Test is not allowed !");
            }


            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    //_db.Categories.Add(obj);
                    //_db.SaveChanges();
                    _unitOfWork.category.Add(obj);
                    _unitOfWork.Save();
                    TempData["Success"] = "Category Created Successfully !";
                }
            }
            else
            {
                ViewData["error"] = "value are not valid";
                TempData["error"] = "Check Details Entered is not vaid !";
                return View(obj);
            }

            //return RedirectToAction("Index" ,"Category");
            return RedirectToAction("Index");

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Category category = _db.Categories.Find(id); 
            Category category = _unitOfWork.category.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {


            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    _unitOfWork.category.Update(obj);
                    _unitOfWork.Save();
                }
            }
            else
            {
                ViewData["error"] = "value are not valid";

                return View(obj);
            }

            //return RedirectToAction("Index" ,"Category");
            return RedirectToAction("Index");

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _unitOfWork.category.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.category.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            //_db.Categories.Remove(category);
            _unitOfWork.category.Remove(category);
            _unitOfWork.Save();

            //return RedirectToAction("Index" ,"Category");
            return RedirectToAction("Index");

        }

    }
}
