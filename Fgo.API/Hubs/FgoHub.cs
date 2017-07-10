using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Fgo.API.Hubs
{
    [HubName("chat")]
    public class FgoHub : Hub
    {
        public  void Hello()
        {
            Clients.All.hello();
        }
    }
}