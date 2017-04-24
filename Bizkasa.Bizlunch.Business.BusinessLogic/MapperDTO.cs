using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bizkasa.Bizlunch.Data.Entities;
using Bizkasa.Bizlunch.Business.Model;

namespace Bizkasa.Bizlunch.Business.BusinessLogic
{
    public interface IMapperDTO
    {
        MapperConfiguration RegisterMap();
    }
    public partial class MapperDTO : IMapperDTO
    {
        #region Constructors

        public MapperDTO()
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        //public void RegisterMap()
        //{
        //    this.RegisterMap2();
        //}
        public MapperConfiguration RegisterMap()
        {
            var m_MapperConfiguration = new MapperConfiguration(configure =>
            {
                //  configure.CreateMissingTypeMaps = true;

                this.RegisterMap_General(configure);
            });

            return m_MapperConfiguration;
        }
        protected void RegisterMap_General(IMapperConfiguration mapper)
        {
            //Create map for member

            #region Account
            mapper.CreateMap<DB_TB_ACCOUNTS, AccountDTO>();
            mapper.CreateMap<AccountDTO, DB_TB_ACCOUNTS>();
            mapper.CreateMap<DB_TB_ACCOUNTS, LoginResultDTO>();


            #endregion

            #region Restaurant
            mapper.CreateMap<DB_TB_RESTAURANT, RestaurantDTO>();
            mapper.CreateMap<RestaurantDTO, DB_TB_RESTAURANT>();
            #endregion

            #region Group

         
            #endregion
        }

        #endregion

        #region Constants

        #endregion
    }
}
