using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace USD_Price
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public double maxPrice;
        public double minPrice;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}

namespace USD_Price.Global.asax
{
    class USD_CBC
    {
        internal static string[] GetPrice()
        {
            string urlAddress = "http://cb.gov.sy/ar/exchange-rate/all?type=100";
            string data = "";
            string HTML;
            using (var wc = new WebClient()) // "using" keyword automatically closes WebClient stream on download completed
            {
                HTML = wc.DownloadString(urlAddress);
            }


            HTML = HTML.Substring(HTML.IndexOf("<tbody>"), 200);
            data = HTML;

            data = data.Substring(data.IndexOf("1</td>"), 100);
            string[] parms = new string[] { "</td><td>" };
            return data.Split(parms, StringSplitOptions.RemoveEmptyEntries);

            
        }
    }



    public static class GlobalVariables
    {

        public static double maxPrice
        {
            get
            {
                return Convert.ToDouble(HttpContext.Current.Application["maxPrice"]);
            }
            set
            {
                HttpContext.Current.Application["maxPrice"] = value;
            }
        }

        public static double minPrice
        {
            get
            {
                return Convert.ToDouble(HttpContext.Current.Application["minPrice"]);
            }
            set
            {
                HttpContext.Current.Application["minPrice"] = value;
            }
        }

        public static string dateUpdate
        {
            get
            {
                return HttpContext.Current.Application["dateUpdate"] as string;
            }
            set
            {
                HttpContext.Current.Application["dateUpdate"] = value;
            }
        }
    }


}