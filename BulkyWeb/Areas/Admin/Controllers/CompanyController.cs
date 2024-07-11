using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
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
    public class CompanyController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        public readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }
        public IActionResult Index()
        {
            //  List<Company> company = _unitOfWork.company.GetAll().ToList();
            //  CompanyVM companyvm = company.ToList();
           // List<Company> company = _unitOfWork.company.GetAll().ToList();
                //.ToList();
            return View();
        }

        #region Ajax API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> company = _unitOfWork.company.GetAll().ToList();
            //.ToList();
            return Json(new { data = company });
        }

        [HttpDelete]
        public IActionResult DeleteAPI(int id)
        {
            var companyToBeDeleted = _unitOfWork.company.Get(u => u.Id == id);

            if(companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deteting Company" });
            }

            _unitOfWork.company.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Json(new {success=true,message="Successfully Deleted Company"});
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
           
            if (id == null || id == 0)
            {              
                return View(new Company());
            }
            else
            {
                Company company = _unitOfWork.company.Get(u => u.Id == id);
                return View (company);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company objCreate)
        {
            

            if (ModelState.IsValid)
            {
               
                if (objCreate.Id == 0 || objCreate.Id == null)
                {
                    _unitOfWork.company.Add(objCreate);
                }
                else
                {
                    _unitOfWork.company.Update(objCreate);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Company Created Successfully !";
            }
            else
            {
               
                TempData["error"] = "Company  Creation Failed!";
                return View(objCreate);
            }
            //return View(Index());
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (id != null)
            {
                var company = _unitOfWork.company.Get(u => u.Id == id);
                if (company != null)
                {
                    return View(company);
                }
                else
                {
                    TempData["error"] = "Error while selecting Company!";
                    return NotFound();
                }

            }
            else
            {
                TempData["error"] = "Error while selecting Company!";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(Company objEdit)
        {
            if (objEdit != null)
            {
                _unitOfWork.company.Update(objEdit);
                _unitOfWork.Save();

                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Error while Deleting Company!";
                return NotFound();
            }
        }

        public IActionResult Delete(int id)
        {
            if (id != null)
            {
                var company = _unitOfWork.company.Get(u => u.Id == id);
                if (company != null)
                {
                    return View(company);
                }
                else
                {
                    TempData["error"] = "Error while selecting Company!";
                    return NotFound();
                }

            }
            else
            {
                TempData["error"] = "Error while selecting Company!";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public IActionResult Delete(Company objDelete)
        {
            try
            {
                if (objDelete != null)
                {
                    _unitOfWork.company.Remove(objDelete);
                    _unitOfWork.Save();

                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["error"] = "Error while Deleting Company!";
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
