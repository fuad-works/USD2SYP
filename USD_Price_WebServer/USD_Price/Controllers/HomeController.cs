using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using USD_Price.Global.asax;
using USD_Price.Models;

namespace USD_Price.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {


            double maxPrice = GlobalVariables.maxPrice;
            double minPrice = GlobalVariables.minPrice;
            ViewBag.maxPrice = maxPrice;
            ViewBag.minPrice = minPrice;
            ViewBag.date = GlobalVariables.dateUpdate;
            //Send Notification to All Clients

            DefaultHubManager hd = new DefaultHubManager(GlobalHost.DependencyResolver);
            var context = GlobalHost.ConnectionManager.GetHubContext<Hubs.uSDHub>();
            context.Clients.All.Send_Price(maxPrice, minPrice, GlobalVariables.dateUpdate);
            return View();
        }

        public ActionResult Price()
        {
            PriceViewModel price = new PriceViewModel();
            price.maxPrice = GlobalVariables.maxPrice;
            price.minPrice = GlobalVariables.minPrice;
            price.UpdateDate = DateTime.Now;
            return View(price);
        }

        [HttpPost]
        public ActionResult Price(PriceViewModel price)
        {
            GlobalVariables.maxPrice = price.maxPrice;
            GlobalVariables.minPrice = price.minPrice;
            GlobalVariables.dateUpdate = DateTime.Now.ToString();
            price.UpdateDate = Convert.ToDateTime(GlobalVariables.dateUpdate);

            DefaultHubManager hd = new DefaultHubManager(GlobalHost.DependencyResolver);
            var context = GlobalHost.ConnectionManager.GetHubContext<Hubs.uSDHub>();
            context.Clients.All.Send_Price(price.maxPrice, price.minPrice, price.UpdateDate);

            Redirect("Index");
            return View(price);
        }


        public ActionResult ShowPrice()
        {
            return View();
        }
    }
}