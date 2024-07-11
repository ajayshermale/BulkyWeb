using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {

        ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
           _db.Update(orderHeader);
        }

        public void UpdateRazorPaymentID(int id, string PaymentIntentId, 
            string TrackingNumber, string OrderStatus, string PaymentStatus, DateTime PaymentDate)
        {
            try
            {
                var orderheaderobj = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
                if (orderheaderobj != null)
                {
                    if (!string.IsNullOrEmpty(PaymentIntentId))
                    {
                        orderheaderobj.PaymentIntenId = PaymentIntentId;
                        orderheaderobj.TrackingNumber = TrackingNumber;
                        orderheaderobj.OrderStatus = OrderStatus;
                        orderheaderobj.PaymentStatus = PaymentStatus;
                        orderheaderobj.PaymentDate = DateTime.Now;
                        _db.OrderHeaders.Update(orderheaderobj);
                        _db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
            
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderheaderobj = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderheaderobj != null)
            {
                orderheaderobj.OrderStatus = orderStatus;
                if (string.IsNullOrEmpty(paymentStatus))
                {
                    orderheaderobj.PaymentStatus = paymentStatus;
                }
                _db.OrderHeaders.Update(orderheaderobj);
                _db.SaveChanges();
            }
        }
    }
}
