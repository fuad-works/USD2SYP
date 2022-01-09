using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using USD_Price.Global.asax;
using USD_Price.Models;

namespace USD_Price.Controllers
{
    public class PriceAPIController : ApiController
    {
        // GET: api/PriceAPI
        public List<PriceViewModel> Get()
        {
            List<PriceViewModel> Ps = new List<PriceViewModel>();

            PriceViewModel price = new PriceViewModel();
            price.maxPrice = GlobalVariables.maxPrice;
            price.minPrice = GlobalVariables.minPrice;
            price.UpdateDate = Convert.ToDateTime(GlobalVariables.dateUpdate);
            Ps.Add(price);

            return Ps;
        }


    }

}
