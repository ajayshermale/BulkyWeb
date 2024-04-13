using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

//using System.Web.Mvc;
//using Microsoft.AspNetCore.MVC.Rendering;

namespace Bulky.Models.ViewModel
{
    public class ProductVM
    {
        public Product Product { get; set; }
        [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
