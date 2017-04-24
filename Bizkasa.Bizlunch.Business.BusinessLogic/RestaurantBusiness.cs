using Bizkasa.Bizlunch.Business.Base;
using Bizkasa.Bizlunch.Business.Extention;
using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Utils;
using Bizkasa.Bizlunch.Data.Entities;
using Bizkasa.Bizlunch.Data.Reponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.BusinessLogic
{
    public interface IRestaurantBusiness
    {
        bool AddOrUpdateRestaurant(RestaurantDTO dto);
        List<RestaurantDTO> GetRestaurants();
        RestaurantDTO GetRestaurant(int restaurantId);
    }
  public  class RestaurantBusiness:BusinessBase, IRestaurantBusiness
    {

        #region Contructors
        public RestaurantBusiness() { }
        #endregion

        #region Properties
        private IBizlunchUnitOfWork UnitOfWork
        {
            get
            {
                return IoC.Get<IBizlunchUnitOfWork>();
            }

        }
        #endregion

        #region Method
        public bool AddOrUpdateRestaurant(RestaurantDTO dto)
        {
            try
            {
                var m_restaurantRepository = UnitOfWork.Repository<DB_TB_RESTAURANT>();
                var m_accountRestaurantRepository = UnitOfWork.Repository<DB_TB_ACCOUNT_RESTAURANT>();
                if (dto.Id > 0)
                {
                    //update
                    var m_restaurant = m_restaurantRepository.Get(a => a.Id == dto.Id);
                    m_restaurant.Name = dto.Name;
                    m_restaurant.MenuUrl = dto.MenuUrl;
                    m_restaurant.Address = dto.MenuUrl;
                    m_restaurant.IsDelivery = dto.IsDelivery;
                    m_restaurantRepository.Update(m_restaurant);
                }
                else
                {
                    var m_restaurant = SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<DB_TB_RESTAURANT>(dto);
                    m_restaurant.CreatedDate = DateTime.Now;
                    m_restaurant.OwnerId = WorkContext.UserContext.UserId;
                    m_restaurantRepository.Add(m_restaurant);
                    m_accountRestaurantRepository.Add(new DB_TB_ACCOUNT_RESTAURANT() {
                        AccountId=m_restaurant.OwnerId,
                        DB_TB_RESTAURANT= m_restaurant,
                        CreatedDate=DateTime.Now
                        
                    });



                }

                UnitOfWork.Commit();
                return !this.HasError;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<RestaurantDTO> GetRestaurants()
        {
            try
            {
                int userId = WorkContext.UserContext.UserId;
                var m_restaurantRepository = UnitOfWork.Repository<DB_TB_RESTAURANT>().GetQueryable();
                var m_restaurants = m_restaurantRepository.Where(a => a.DB_TB_ACCOUNT_RESTAURANT.Any(b => b.AccountId == userId)).Select(a => new RestaurantDTO()
                {
                    Address = a.Address,
                    Name = a.Name,
                    MenuUrl = a.MenuUrl,
                    Id = a.Id,
                    Phone = a.Phone,
                    IsDelivery = a.IsDelivery
                }).ToList();
                return m_restaurants;
            }
            catch (Exception)
            {

                return null;
            }
           
        }


        public RestaurantDTO GetRestaurant(int restaurantId)
        {
            var m_restaurantRepository = UnitOfWork.Repository<DB_TB_RESTAURANT>();
            var m_restaurant = m_restaurantRepository.Get(a => a.Id == restaurantId);
            return SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<RestaurantDTO>(m_restaurant);
        }

        #endregion
    }
}
