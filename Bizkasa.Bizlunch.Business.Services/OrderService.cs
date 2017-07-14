using Bizkasa.Bizlunch.Business.Base;
using Bizkasa.Bizlunch.Business.BusinessLogic;
using Bizkasa.Bizlunch.Business.Extention;
using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Services
{
    public interface IOrderService
    {
        Response<OrderViewDTO> GetOrderBy(SearchDTO request);
        Response<List<OrderDTO>> GetOrders(SearchDTO request);
        
        Response<OrderViewDTO> AddOrderDetail(OrderDTO dto);
        Response<bool> AddInvite(InviteDTO request);
        Response<bool> AddMoreFriend(InviteMoreFriendDTO request);
        Response<MessageReceiveRow> AddMessageChat(MessageRow request);
        Response<List<MessageReceiveRow>> GetMessageChat(MessageRow request);
    }
    public partial class BizlunchService
    {
        public Response<List<MessageReceiveRow>> GetMessageChat(MessageRow request)
        {
            List < MessageReceiveRow > result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().GetMessageChat(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<MessageReceiveRow> AddMessageChat(MessageRow request)
        {
            MessageReceiveRow result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().AddMessageChat(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<bool> AddMoreFriend(InviteMoreFriendDTO request)
        {
            bool result = false;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().AddMoreFriend(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<bool> AddInvite(InviteDTO request)
        {
            bool result = false;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().AddInvite(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<OrderViewDTO> GetOrderBy(SearchDTO request)
        {
            OrderViewDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().GetOrderBy(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<List<OrderDTO>> GetOrders(SearchDTO request)
        {
            List<OrderDTO> result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().GetOrders(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<OrderViewDTO> AddOrderDetail(OrderDTO dto)
        {
            OrderViewDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().AddOrderDetail(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
       
    }
}
