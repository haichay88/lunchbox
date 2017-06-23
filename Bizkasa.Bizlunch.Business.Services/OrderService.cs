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
        Response<List<OrderDTO>> GetOrders(BaseRequest request);
        Response<bool> AddOrUpdateOrder(OrderDTO dto);
        Response<OrderViewDTO> AddOrderDetail(OrderDTO dto);
        Response<bool> AddInvite(InviteDTO request);
        Response<bool> AddMoreFriend(InviteMoreFriendDTO request);

    }
    public partial class BizlunchService
    {
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

        public Response<List<OrderDTO>> GetOrders(BaseRequest request)
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
        public Response<bool>  AddOrUpdateOrder(OrderDTO dto)
        {
            bool result = false;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().AddOrUpdateOrder(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
    }
}
