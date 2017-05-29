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
    public interface IRestaurantService
    {
        Response<RestaurantDTO> GetRestaurant(int restaurantId);
        Response<List<RestaurantDTO>> GetRestaurants(BaseRequest request);
        Response<bool> AddOrUpdateRestaurant(RestaurantDTO dto);
    }
    public partial class BizlunchService
    {
        public Response<RestaurantDTO> GetRestaurant(int restaurantId)
        {
            RestaurantDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IRestaurantBusiness>().GetRestaurant(restaurantId);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<List<RestaurantDTO>> GetRestaurants(BaseRequest request)
        {
            List<RestaurantDTO> result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IRestaurantBusiness>().GetRestaurants(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<bool> AddOrUpdateRestaurant(RestaurantDTO dto)
        {
            bool result = false;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IRestaurantBusiness>().AddOrUpdateRestaurant(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
       
    }
}
