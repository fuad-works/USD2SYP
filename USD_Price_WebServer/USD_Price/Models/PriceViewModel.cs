using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace USD_Price.Models
{
    public class PriceViewModel
    {
        [Display(Name ="المبيع")]
        public double maxPrice { get; set; }
        [Display(Name = "الشراء")]
        public double minPrice { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}