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
    class BaseInfo
    {
        static private string base_url = "http://uusdprice1-001-site1.ctempurl.com/";

        static public string Base_Url
        {
            get { return base_url; }
            set { base_url = value; }
        }
    }
}