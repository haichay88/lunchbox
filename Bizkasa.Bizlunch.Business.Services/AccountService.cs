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
    public interface IAccountService
    {
        Response<LoginResultDTO> Login(LoginDTO dto);
        Response<bool> RegisterAccount(AccountDTO accountId);
        Response<List<AccountDTO>> GetUsers();
        Response<LoginResultDTO> Relogin(string email);
        Response<AccountDTO> GetUser(int userId);
        Response<AccountDTO> GetUser(string token);
        Response<List<AccountDTO>> AddOrUpdateAccount(AccountDTO dto);
        Response<bool> ConfirmedUser(AccountDTO dto);
        Response<LoginResultDTO> Relogin(AccountDTO dto);
        Response<IList<FriendDTO>> GetFriends(SearchDTO request);
    }
  public partial  class BizlunchService
    {
        public Response<IList<FriendDTO> >GetFriends(SearchDTO request)
        {

            IList<FriendDTO> result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().GetFriends(request);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<bool> RegisterAccount(AccountDTO dto)
        {

            bool result = false;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().RegisterAccount(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<bool> ConfirmedUser(AccountDTO dto)
        {

            bool result = false;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().ConfirmedUser(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<LoginResultDTO> Login(LoginDTO dto)
        {

            LoginResultDTO result =null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().Login(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<LoginResultDTO> Relogin(string email)
        {

            LoginResultDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().Relogin(email);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<LoginResultDTO> Relogin(AccountDTO dto)
        {

            LoginResultDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().Relogin(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<List<AccountDTO>> GetUsers()
        {

            List<AccountDTO> result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().GetUsers();
            });

            return BusinessProcess.Current.ToResponse(result);
        }


        public Response<List<AccountDTO>> AddOrUpdateAccount(AccountDTO dto)
        {

            List<AccountDTO> result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().AddOrUpdateAccount(dto);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

        public Response<AccountDTO> GetUser(int userId)
        {

            AccountDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().GetUser(userId);
            });

            return BusinessProcess.Current.ToResponse(result);
        }
        public Response<AccountDTO> GetUser(string token)
        {

            AccountDTO result = null;
            BusinessProcess.Current.Process(p =>
            {
                result = IoC.Get<IAccountBusiness>().GetUser(token);
            });

            return BusinessProcess.Current.ToResponse(result);
        }

      
        

    }
}
