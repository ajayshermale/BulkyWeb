using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    public class ShoppingCartVM
    {
       public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

       //public double OrderTotal {  get; set; }
       public OrderHeader OrderHeader { get; set; }
        public double Price { get; set; }
    }
}
