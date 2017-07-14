using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;

namespace Fgo.API.Hubs
{
    [HubName("chat")]
    public class FgoHub : Hub
    {
        private IBizlunchService _sevice = new BizlunchService();

        public  void Hello()
        {
            Clients.All.hello();
        }
        public void SendMessage(MessageRow request)
        {
            Clients.Caller.setInitial();
            var result = _sevice.AddMessageChat(request);
            Clients.Caller.SendComplete(result);
            
        }
    }
}