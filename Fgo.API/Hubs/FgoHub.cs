using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using System.Threading.Tasks;
using log4net;

namespace Fgo.API.Hubs
{
    [HubName("chat")]
    public class FgoHub : Hub
    {
        private IBizlunchService _sevice = new BizlunchService();
        private static readonly log4net.ILog logger =
    LogManager.GetLogger(typeof(FgoHub));
        public  void Hello()
        {
            Clients.All.hello();
        }
        public void SendMessage(MessageRow request)
        {

            var result = _sevice.AddMessageChat(request);

            var context = GlobalHost.ConnectionManager.GetHubContext<FgoHub>();
            context.Clients.Group(request.InviteId.ToString()).SendComplete(result);
            


        }
        public void Login(SearchDTO request)
        {
            var result = _sevice.GetGroupChatBy(request);
            if (result.Data.Any())
            {
                foreach (var item in result.Data)
                {
                    Groups.Add(Context.ConnectionId, item);
                }
            }
        }
       

        public Task JoinGroup(string groupName)
        {
            logger.Info("JoinGroup" +Context.ConnectionId);
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }
    }
}