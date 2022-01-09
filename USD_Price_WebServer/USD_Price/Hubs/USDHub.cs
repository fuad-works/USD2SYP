using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace USD_Price.Hubs
{

    public class uSDHub : Hub
    {

        public void Send_Price(double maxPrice, double minPrice,string date)
        {
            Clients.All.Send_Price(maxPrice, minPrice, date);
        }

    }
}