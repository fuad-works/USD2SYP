using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace USD_Client_APP.Models
{
    class Price
    {
        double mp = 0;
        double ip = 0;
        string dat1 = "";
        public double maxPrice { get { return mp; } set { mp = value; } }
        public double minPrice { get { return ip; } set { ip = value; } }
        public string UpdateDate { get { return dat1; } set { dat1 = value; } }


        public override string ToString()
        {
            return maxPrice + " : " + minPrice + " #" + UpdateDate + "#";
        }
    }
}