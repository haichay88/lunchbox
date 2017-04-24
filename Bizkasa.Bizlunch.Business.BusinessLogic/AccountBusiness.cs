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
    public interface IAccountBusiness
    {
        bool RegisterAccount(AccountDTO dto);
        LoginResultDTO Login(LoginDTO dto);
        List<AccountDTO> GetUsers();
        LoginResultDTO Relogin(string email);
        LoginResultDTO Relogin(AccountDTO dto);
        List<AccountDTO> AddOrUpdateAccount(AccountDTO dto);
        AccountDTO GetUser(int userId);
       
        AccountDTO GetUser(string token);
        bool ConfirmedUser(AccountDTO dto);
    }

    public class AccountBusiness : BusinessBase, IAccountBusiness
    {
        #region Contructor
        public AccountBusiness()
        {

        }

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

        #region Methods
        public bool RegisterAccount(AccountDTO dto)
        {
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            if (dto.Id > 0)
            {
                // update 
                var m_account = m_accountRepository.Get(a => a.ACC_SYS_ID == dto.Id);
                m_account.ACC_LASTNAME = dto.LastName;
                m_account.ACC_FIRSTNAME = dto.FirstName;
                if (!string.IsNullOrWhiteSpace(dto.Password))
                    m_account.ACC_PASSWORD = dto.Password;
                m_accountRepository.Update(m_account);
            }
            else
            {
                //add
                var m_account = SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<DB_TB_ACCOUNTS>(dto);
                m_account.ACC_TOKEN = Guid.NewGuid().ToString();
                if (IsExistAccount(m_account.ACC_EMAIL))
                {
                    base.AddError("Tai khoan da ton tai");
                    return false;
                }
                m_accountRepository.Add(m_account);

            }
            UnitOfWork.Commit();
            return false;

        }
        public LoginResultDTO Relogin(AccountDTO dto)
        {
           
            if (string.IsNullOrWhiteSpace(dto.Email))
                return null;
            var m_accountRepository= UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            if (!IsExistAccount(dto.Email))
            {
                var m_account = SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<DB_TB_ACCOUNTS>(dto);
                m_account.ACC_TOKEN = Guid.NewGuid().ToString();
                m_accountRepository.Add(m_account);
                UnitOfWork.Commit();
            }
           
            return m_accountRepository.GetQueryable()
                           .Where(a => a.ACC_EMAIL == dto.Email)
                           .Select(a => new LoginResultDTO()
                           {
                               Email = a.ACC_EMAIL,
                               FirstName = a.ACC_FIRSTNAME,
                               LastName = a.ACC_LASTNAME,
                               Id = a.ACC_SYS_ID,
                               Token = a.ACC_TOKEN
                           }).FirstOrDefault();
        }

        public LoginResultDTO Login(LoginDTO dto)
        {
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            if (!IsExistAccount(dto.Email))
            {
                base.AddError("Tai khoan khong ton tai");
                return null;
            }
            var m_account = m_accountRepository.Get(a => a.ACC_EMAIL == dto.Email && a.ACC_PASSWORD == dto.Password);
            if (m_account == null)
            {
                base.AddError("Mat khau khong dung");
                return null;
            }
          
            m_account.ACC_TOKEN = Guid.NewGuid().ToString();

            m_accountRepository.Update(m_account);

            UnitOfWork.Commit();

            return SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<LoginResultDTO>(m_account);


        }

        public LoginResultDTO Relogin(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            return UnitOfWork.Repository<DB_TB_ACCOUNTS>().GetQueryable()
                .Where(a => a.ACC_EMAIL == email)
                .Select(a => new LoginResultDTO()
                {
                    Email = a.ACC_EMAIL,
                    FirstName = a.ACC_FIRSTNAME,
                    LastName = a.ACC_LASTNAME,
                    Id = a.ACC_SYS_ID,
                    Token = a.ACC_TOKEN
                }).FirstOrDefault();
        }
        public bool IsExistAccount(string email)
        {

            return UnitOfWork.Repository<DB_TB_ACCOUNTS>().Get(a => a.ACC_EMAIL == email) != null;

        }


        public List<AccountDTO> GetUsers()
        {
            try
            {
                var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
                var m_users = m_accountRepository.GetQueryable().Where(a => a.DB_TB_FRIENDSHIP1.Any(b => b.AccountId == WorkContext.UserContext.UserId))
                    .Select(a => new AccountDTO()
                    {
                        Id=a.ACC_SYS_ID,
                        Email = a.ACC_EMAIL,
                        FirstName = a.ACC_FIRSTNAME,
                        LastName = a.ACC_LASTNAME
                    }).ToList();
                return m_users;
            }
            catch (Exception)
            {
                return null;
                
            }
          
            
        }

        public AccountDTO GetUser(int userId)
        {
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            var m_myAccount = m_accountRepository.Get(a => a.ACC_SYS_ID == userId && a.ACC_IS_ACTIVED);
            return SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<AccountDTO>(m_myAccount);
        }

        public AccountDTO GetUser(string token)
        {
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            var m_myAccount = m_accountRepository.Get(a => a.ACC_TOKEN == token);
            return SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<AccountDTO>(m_myAccount);
        }
        public bool ConfirmedUser(AccountDTO dto)
        {
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            var m_account = m_accountRepository.Get(a => a.ACC_TOKEN == dto.Token);
            m_account.ACC_PASSWORD = dto.Password;
            m_account.ACC_IS_ACTIVED = true;
            m_accountRepository.Update(m_account);
            UnitOfWork.Commit();
            return !this.HasError;
        }
        public List<AccountDTO> AddOrUpdateAccount(AccountDTO dto)
        {
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
           
            if (dto.Id > 0)
            {
                // update 
                var m_account = m_accountRepository.Get(a => a.ACC_SYS_ID == dto.Id);
                m_account.ACC_LASTNAME = dto.LastName;
                m_account.ACC_FIRSTNAME = dto.FirstName;

                m_accountRepository.Update(m_account);
            }
            else
            {
                //add
                var m_account = SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<DB_TB_ACCOUNTS>(dto);
                m_account.ACC_TOKEN = Guid.NewGuid().ToString();
                if (IsExistAccount(m_account.ACC_EMAIL))
                {
                    base.AddError("Tai khoan da ton tai");
                    return null;
                }
               
                m_account.ACC_OWNER_ID = WorkContext.UserContext.UserId;
               
                m_accountRepository.Add(m_account);
                var m_friendRepository = UnitOfWork.Repository<DB_TB_FRIENDSHIP>();
                m_friendRepository.Add(new DB_TB_FRIENDSHIP() {
                    AccountId= WorkContext.UserContext.UserId,
                    FriendId= m_account.ACC_SYS_ID,
                    CreatedDate=DateTime.Now
                });

            }
            UnitOfWork.Commit();
            return GetUsers();

        }

      
       
        #endregion
    }
}
