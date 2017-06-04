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
        Response<OrderViewDTO> GetOrderBy(int orderId);
        Response<List<OrderDTO>> GetOrders(BaseRequest request);
        Response<bool> AddOrUpdateOrder(OrderDTO dto);
        Response<OrderViewDTO> AddOrderDetail(OrderDTO dto);

    }
    public partial class BizlunchService
    {
        public Response<OrderViewDTO> GetOrderBy(int orderId)
        {
            OrderViewDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IOrderBusiness>().GetOrderBy(orderId);
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
