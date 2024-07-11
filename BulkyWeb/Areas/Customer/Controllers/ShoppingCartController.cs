using Bulky.DataAccess.Migrations;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using System.Collections.Generic;
using System.Security.Claims;
using Bulky.Models.Models;
using Razorpay.Api;
using System.Reflection.Metadata;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace BulkyWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class ShoppingCartController : Controller
	{
		IUnitOfWork _UnitOfWork;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		[BindProperty]
		public OrderDetail OrderDetail { get; set; }

		public ShoppingCartController(IUnitOfWork unitOfWork)
		{
			_UnitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var UserID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


			ShoppingCartVM = new()
			{
				ShoppingCartList = _UnitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == UserID, includeProperties: "Product"),
				OrderHeader = new()
			};

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.price = GetPriceBasedOnQuantity(cart);

				ShoppingCartVM.OrderHeader.OrderTotal += (cart.price * cart.Count);

				foreach (var cartvm in ShoppingCartVM.ShoppingCartList.Where((x => x.Id == cart.Id)))
				{
					cartvm.Product.Price = cart.price;
				}
			}

			return View(ShoppingCartVM);
		}

		public IActionResult Plus(int CartId)
		{
			var cartobj = _UnitOfWork.shoppingCart.Get(x => x.Id == CartId);
			if (cartobj != null)
			{
				cartobj.Count += 1;
				_UnitOfWork.shoppingCart.Update(cartobj);
				_UnitOfWork.Save();
			}
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int CartId)
		{
			var cartobj = _UnitOfWork.shoppingCart.Get(x => x.Id == CartId);
			if (cartobj != null)
			{
				if (cartobj.Count <= 1)
				{
					_UnitOfWork.shoppingCart.Remove(cartobj);
				}
				else
				{
					cartobj.Count -= 1;
					_UnitOfWork.shoppingCart.Update(cartobj);
				}
				_UnitOfWork.Save();
			}
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cartobj = _UnitOfWork.shoppingCart.Get(x => x.Id == cartId);
			if (cartobj != null)
			{
				_UnitOfWork.shoppingCart.Remove(cartobj);
			}
			_UnitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var UserID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


			ShoppingCartVM = new()
			{
				ShoppingCartList = _UnitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == UserID, includeProperties: "Product"),
				OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = _UnitOfWork.applicationUser.Get(x => x.Id == UserID);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.PostalCode;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.price = GetPriceBasedOnQuantity(cart);

				ShoppingCartVM.OrderHeader.OrderTotal += (cart.price * cart.Count);

				foreach (var cartvm in ShoppingCartVM.ShoppingCartList.Where((x => x.Id == cart.Id)))
				{
					cartvm.Product.Price = cart.price;
				}
			}
			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var UserID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


			ShoppingCartVM.ShoppingCartList = _UnitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == UserID, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = UserID;


			Bulky.Models.Models.ApplicationUser applicationUser = _UnitOfWork.applicationUser.Get(x => x.Id == UserID);

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.price * cart.Count);
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//if 0 then it is regular customer
				ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			}
			else
			{   //it is company user payment is delay for 30 days
				ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
			}

			_UnitOfWork.orderHeader.Add(ShoppingCartVM.OrderHeader);
			_UnitOfWork.Save();

			double priceobj;
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				
				OrderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.price,
					Count = cart.Count
				};
				priceobj= OrderDetail.Price;
				_UnitOfWork.orderDetail.Add(OrderDetail);
				_UnitOfWork.Save();
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//it is a regular customer account and we need to capture payment
				//stripe logic //Razor logic
				Dictionary<string, object> input = new Dictionary<string, object>();

				input.Add("amount", (long)(OrderDetail.Price * 100)); // this amount should be same as transaction amount
				input.Add("currency", "INR");
				input.Add("receipt", "12121");


				string key = "rzp_test_q8ReKt8vgaBd1k";
				string secret = "vXNraKAtQtJ6G8kcbGf173yW";
				RazorpayClient client = new RazorpayClient(key, secret);

				Razorpay.Api.Order order = client.Order.Create(input);
				var orderId = order["id"].ToString();
                TempData["orderIdobj"] = order["id"].ToString();
                ShoppingCartVM.OrderHeader.PaymentIntentId = orderId;
				//TempData["orderIdobj"] = orderId; 
                //ViewBag.OrderId = orderId; 
            }
			//return View(ShoppingCartVM);
			return RedirectToAction(nameof(OrderConfirmation),new {id=ShoppingCartVM.OrderHeader.Id});
		}

		public IActionResult OrderConfirmation(int id, string orderIdobj)
		{
            ViewBag.OrderId = TempData["orderIdobj"];
            OrderHeader orderHeader = _UnitOfWork.orderHeader.Get(x => x.Id == id);
			return View(orderHeader);
		}

		[HttpPost]
		public IActionResult Payment(int id,string razorpay_payment_id , string razorpay_order_id,
			string razorpay_signature)
        {
            var orderHeader = _UnitOfWork.orderHeader.Get(x => x.Id == id);
            Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);

			Utils.verifyPaymentSignature(attributes);

            //OrderHeader orderHeader = new OrderHeader();
            orderHeader.PaymentIntentId = razorpay_payment_id.ToString();
			orderHeader.TrackingNumber = razorpay_order_id.ToString();
			orderHeader.OrderStatus = SD.StatusApproved;
			orderHeader.PaymentStatus = SD.PaymentStatusApproved;
			orderHeader.PaymentDate = System.DateTime.Now;
			_UnitOfWork.orderHeader.UpdateRazorPaymentID(orderHeader.Id, orderHeader.PaymentIntentId, orderHeader.TrackingNumber,
            orderHeader.OrderStatus = SD.StatusApproved, orderHeader.PaymentStatus = SD.PaymentStatusApproved,
            orderHeader.PaymentDate = System.DateTime.Now);

			List<ShoppingCart> shoppingCarts = _UnitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_UnitOfWork.shoppingCart.RemoveRange(shoppingCarts);
			_UnitOfWork.Save();

            return View(orderHeader);
		}

		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{

			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else
			{
				if (shoppingCart.Count <= 100)
				{
					return shoppingCart.Product.Price50;
				}
				else
				{
					return shoppingCart.Product.Price100;
				}
			}
		}

	}
}
