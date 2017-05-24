using Bizkasa.Bizlunch.Business.Base;
using Bizkasa.Bizlunch.Business.BusinessLogic;
using Bizkasa.Bizlunch.Business.Services;
using Bizkasa.Bizlunch.Data.Core;
using Bizkasa.Bizlunch.Data.Reponsitory;
using Fgo.API.Infractstructure;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Fgo.API.App_Start
{
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterType<BusinessProcess>(new HttpContextLifetimeManager<BusinessProcess>());

            #region service
            container.RegisterType<IBizlunchService, BizlunchService>(new HttpContextLifetimeManager<IBizlunchService>());
            #endregion

            #region Reponsitory
            container.RegisterType<IUnitOfWork, EFUnitOfWork>(new HttpContextLifetimeManager<IUnitOfWork>());
            container.RegisterType<IBizlunchUnitOfWork, BizlunchUnitOfWork>(new HttpContextLifetimeManager<IBizlunchUnitOfWork>());
            #endregion

            #region Business
            container.RegisterType<IAccountBusiness, AccountBusiness>();
            container.RegisterType<IRestaurantBusiness, RestaurantBusiness>();
            container.RegisterType<IOrderBusiness, OrderBusiness>();
            #endregion
            #region Other

            #endregion
        }
    }
}