using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        IUnitOfWork _unitOfWork;

        [BindProperty]
       public OrderVM orderVM  { get; set;}

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            orderVM = new()
            {
                orderheader = _unitOfWork.orderHeader.Get(x => x.Id == id,includeProperties:"ApplicationUser"),
                orderDetails = _unitOfWork.orderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles=SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var orderVMobjDB = _unitOfWork.orderHeader.Get(x => x.Id == orderVM.orderheader.Id);
            orderVMobjDB.Name = orderVM.orderheader.Name;
            orderVMobjDB.PhoneNumber = orderVM.orderheader.PhoneNumber;
            orderVMobjDB.StreetAddress = orderVM.orderheader.StreetAddress;
            orderVMobjDB.City = orderVM.orderheader.City;
            orderVMobjDB.State = orderVM.orderheader.State;
            orderVMobjDB.PostalCode = orderVM.orderheader.PostalCode;

            if (!orderVM.orderheader.Carrier.IsNullOrEmpty())
            {
                orderVMobjDB.Carrier = orderVM.orderheader.Carrier;
            }
            if (!orderVM.orderheader.TrackingNumber.IsNullOrEmpty())
            {
                orderVMobjDB.TrackingNumber = orderVM.orderheader.TrackingNumber;
            }
            _unitOfWork.orderHeader.Update(orderVMobjDB);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Saved Successfully!";

            return RedirectToAction(nameof(Details),new { id = orderVMobjDB.Id});
        }


        [HttpGet]
        public IActionResult GetAll(string status)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var UserID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<OrderHeader> ordersobj;
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                ordersobj = _unitOfWork.orderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                ordersobj = _unitOfWork.orderHeader.GetAll( x => x.ApplicationUserId == userId ,includeProperties: "ApplicationUser").ToList();
            }


            switch (status)
            {
                case "inprocess" :
                 ordersobj = ordersobj.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "pending":
                    ordersobj = ordersobj.Where(u => u.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    ordersobj = ordersobj.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    ordersobj = ordersobj.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }       

            return Json(new { data = ordersobj });
        }

        [HttpPost]
        [Authorize(Roles=SD.Role_Admin +","+SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.orderHeader.UpdateStatus(orderVM.orderheader.Id, SD.StatusInProcess);
            //_unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { id = orderVM.orderheader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            _unitOfWork.orderHeader.UpdateStatus(orderVM.orderheader.Id, SD.StatusShipped);
            //_unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { id = orderVM.orderheader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            _unitOfWork.orderHeader.UpdateStatus(orderVM.orderheader.Id, SD.StatusCancelled);
            //_unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { id = orderVM.orderheader.Id });
        }
    }
}
