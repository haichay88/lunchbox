using Bizkasa.Bizlunch.Business.Base;
using Bizkasa.Bizlunch.Business.Extention;
using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Utils;
using Bizkasa.Bizlunch.Data.Entities;
using Bizkasa.Bizlunch.Data.Reponsitory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    
        IList<FriendDTO> GetFriends(SearchDTO request);
        List<AccountDTO> AddOrUpdateFriend(AccountDTO dto);
        LoginResultDTO SignUp(SignUpDTO request);
        IList<FriendDTO> SyncFriends(InviteMoreFriendDTO request);
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

                //m_account.ACC_TOKEN = Guid.NewGuid().ToString();
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
                base.AddError("Account is not existed !");
                return null;
            }
            var m_account = m_accountRepository.Get(a => a.ACC_EMAIL == dto.Email && a.ACC_PASSWORD == dto.Password && a.ACC_IS_ACTIVED);
            if (m_account == null)
            {
                base.AddError("Password incorrect!");
                return null;
            }
            //if (string.IsNullOrEmpty(m_account.ACC_TOKEN))
            //{
            //    // ma hoa thong tin dang nhap
            //    ContextDTO context = new ContextDTO() {
            //        Id=m_account.ACC_SYS_ID,
            //        Email=m_account.ACC_EMAIL,
            //        FirstName=m_account.ACC_FIRSTNAME,
            //        LastName=m_account.ACC_LASTNAME
            //    };
            //    m_account.ACC_TOKEN =EncryptDecryptUtility.Encrypt(XmlUtility.Serialize(context),true);
            //}
            ContextDTO context = new ContextDTO()
            {
                Id = m_account.ACC_SYS_ID,
                Email = m_account.ACC_EMAIL,
                FirstName = m_account.ACC_FIRSTNAME,
                LastName = m_account.ACC_LASTNAME
            };
            m_account.ACC_RESGISTRANTION_ID = dto.DeviceKey;
            m_account.ACC_LASTLOGIN_DATE = DateTime.Now;
            if(string.IsNullOrEmpty(m_account.ACC_TOKEN))
                m_account.ACC_TOKEN = EncryptDecryptUtility.Encrypt(XmlUtility.Serialize(context), true);

            m_accountRepository.Update(m_account);

            UnitOfWork.Commit();

            return new LoginResultDTO() {
                Email=m_account.ACC_EMAIL,
                Id=m_account.ACC_SYS_ID,
                Token=m_account.ACC_TOKEN,
                FirstName=m_account.ACC_FIRSTNAME,
                LastName=m_account.ACC_LASTNAME
            };// SingletonAutoMapper._Instance.MapperConfiguration.CreateMapper().Map<LoginResultDTO>(m_account);


        }

        public LoginResultDTO Relogin(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            var m_account = m_accountRepository.Get(a => a.ACC_EMAIL == email);
            ContextDTO context = new ContextDTO()
            {
                Id = m_account.ACC_SYS_ID,
                Email = m_account.ACC_EMAIL,
                FirstName = m_account.ACC_FIRSTNAME,
                LastName = m_account.ACC_LASTNAME
            };
            m_account.ACC_TOKEN = EncryptDecryptUtility.Encrypt(XmlUtility.Serialize(context), true);

            m_accountRepository.Update(m_account);

            UnitOfWork.Commit();
            return new LoginResultDTO()
            {
                Email = m_account.ACC_EMAIL,
                Id = m_account.ACC_SYS_ID,
                Token = m_account.ACC_TOKEN,
                FirstName = m_account.ACC_FIRSTNAME,
                LastName = m_account.ACC_LASTNAME
            };
            
        }
        public bool IsExistAccount(string email)
        {

            return UnitOfWork.Repository<DB_TB_ACCOUNTS>().Get(a => a.ACC_EMAIL == email) != null;

        }
       
        public LoginResultDTO SignUp(SignUpDTO request)
        {
            if(string.IsNullOrEmpty(request.Email))
            {
                base.AddError("Please input Email");
                return null;
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                base.AddError("Please input Password");
                return null;
            }
            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            var emailExist = m_accountRepository.GetQueryable().Where(a => a.ACC_EMAIL == request.Email ).FirstOrDefault();
            if (emailExist != null)
            {
                if (emailExist.ACC_IS_ACTIVED)
                {
                    base.AddError("Email existed !");
                    return null;
                }
                else
                {
                    emailExist.ACC_IS_ACTIVED = true;
                    emailExist.ACC_PASSWORD = request.Password;
                    m_accountRepository.Update(emailExist);

                }

            }
            else
            {
                var newAccount = new DB_TB_ACCOUNTS()
                {
                    ACC_EMAIL = request.Email,
                    ACC_PASSWORD = request.Password,
                    ACC_IS_ACTIVED = true,
                    ACC_RESGISTRANTION_ID = request.DeviceKey,

                };

                m_accountRepository.Add(newAccount);
            }

           
            UnitOfWork.Commit();

            return Relogin(request.Email);
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
            // check authenticate
            if (dto.Context == null)
            {
                base.AddError("Authenticate failed !");
                return null;
            }

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
                var m_account = new DB_TB_ACCOUNTS() {
                    ACC_EMAIL=dto.Email,
                    ACC_FIRSTNAME=dto.FirstName,
                    ACC_MIDDLENAME=dto.MiddleName,
                    ACC_IS_ACTIVED=false,
                    ACC_LASTNAME=dto.LastName,
                    ACC_OWNER_ID=dto.Context.Id,
                    ACC_PASSWORD=dto.Password,
                   
                };
                //m_account.ACC_TOKEN = Guid.NewGuid().ToString();
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

        public List<AccountDTO> AddOrUpdateFriend(AccountDTO dto)
        {
            // check authenticate
            if (dto.Context == null)
            {
                base.AddError("Authenticate failed !");
                return null;
            }

            var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            var m_friendRepository = UnitOfWork.Repository<DB_TB_FRIENDSHIP>();
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
                var m_account = new DB_TB_ACCOUNTS()
                {
                    ACC_EMAIL = dto.Email,
                    ACC_FIRSTNAME = dto.FirstName,
                    ACC_MIDDLENAME = dto.MiddleName,
                    ACC_IS_ACTIVED = false,
                    ACC_LASTNAME = dto.LastName,
                    ACC_OWNER_ID = dto.Context.Id,                   

                };
                // check email alright exist?
                var accountExist = m_accountRepository.GetQueryable().Where(a => a.ACC_EMAIL == m_account.ACC_EMAIL).FirstOrDefault();

                if (accountExist != null)
                {
                    // check relationship if account existed
                    var friendRelation = m_friendRepository.GetQueryable().Where(a => a.AccountId == dto.Context.Id && a.FriendId == accountExist.ACC_SYS_ID).FirstOrDefault();
                    if (friendRelation == null)
                    {
                        m_friendRepository.Add(new DB_TB_FRIENDSHIP()
                        {
                            AccountId =dto.Context.Id,
                            FriendId = accountExist.ACC_SYS_ID,
                            CreatedDate = DateTime.Now
                        });
                    }

                }
                else
                {
                    m_account.ACC_OWNER_ID = dto.Context.Id;
                    m_accountRepository.Add(m_account);
                    m_friendRepository.Add(new DB_TB_FRIENDSHIP()
                    {
                        AccountId = dto.Context.Id,
                        FriendId = m_account.ACC_SYS_ID,
                        CreatedDate = DateTime.Now
                    });
                }

            }
            UnitOfWork.Commit();
            return GetUsers();

        }



        /// <summary>
        /// Get friends of current user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IList<FriendDTO> GetFriends(SearchDTO request)
        {
            if (request.Context == null)
            {
                base.AddError("Authenticate failed !");
                return null;
            }
            try
            {
                var m_friendshipRepository = UnitOfWork.Repository<DB_TB_FRIENDSHIP>();
                var m_friendshipQueryable = m_friendshipRepository.GetQueryable().Where(a => a.AccountId == request.Context.Id );
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    m_friendshipQueryable = m_friendshipQueryable.Where(a => a.DB_TB_ACCOUNTS1.ACC_EMAIL.Contains(request.Keyword)
                     || a.DB_TB_ACCOUNTS1.ACC_FIRSTNAME.Contains(request.Keyword)
                     || a.DB_TB_ACCOUNTS1.ACC_LASTNAME.Contains(request.Keyword)
                     || a.DB_TB_ACCOUNTS1.ACC_FIRSTNAME.Contains(request.Keyword)
                     || a.DB_TB_ACCOUNTS1.ACC_MIDDLENAME.Contains(request.Keyword));
                }
                var m_frship = m_friendshipQueryable.Select(a => new FriendDTO()
                {
                    Email = a.DB_TB_ACCOUNTS1.ACC_EMAIL,
                    Id = a.DB_TB_ACCOUNTS1.ACC_SYS_ID,
                    FirstName = a.DB_TB_ACCOUNTS1.ACC_FIRSTNAME,
                    LastName = a.DB_TB_ACCOUNTS1.ACC_LASTNAME,
                    IsActived=a.DB_TB_ACCOUNTS1.ACC_IS_ACTIVED
                }).ToList();

                return m_frship;
            }
            catch (Exception)
            {
                return null;

            }
        }

        public IList<FriendDTO> SyncFriends(InviteMoreFriendDTO request)
        {
            if (request.Context == null)
            {
                base.AddError("Authenticate failed !");
                return null;
            }
            if (request.Friends == null)
            {
                base.AddError("No contact to sync !");
                return null;
            }
            try
            {
                var m_accountRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
                var m_friendshipRepository = UnitOfWork.Repository<DB_TB_FRIENDSHIP>();
                var IorderBusiness = IoC.Get<IOrderBusiness>();
                foreach (var item in request.Friends)
                {
                    var checkAccount = m_accountRepository.GetQueryable().Where(a => a.ACC_EMAIL == item.Email).FirstOrDefault();
                    if (checkAccount != null)
                    {
                       if( !IorderBusiness.CheckExistFriendShip(request.Context.Id, checkAccount.ACC_SYS_ID)) {
                            m_friendshipRepository.Add(IorderBusiness.BuildRowFriendShip(request.Context.Id, checkAccount.ACC_SYS_ID));
                            m_friendshipRepository.Add(IorderBusiness.BuildRowFriendShip(checkAccount.ACC_SYS_ID, request.Context.Id));

                        }
                    }
                    else
                    {
                        var rowAccount = new DB_TB_ACCOUNTS()
                        {
                            ACC_EMAIL=item.Email,
                            ACC_FIRSTNAME=item.FirstName,
                           ACC_IS_ACTIVED=false
                        };
                        m_accountRepository.Add(rowAccount);
                    }
                    UnitOfWork.Commit();
                }
                SearchDTO search = new SearchDTO()
                {
                    Token = request.Token
                };
                return GetFriends(search);

            }
            catch (Exception)
            {
                return null;

            }
        }

        public void SendMessage(NotificationDTO request)
        {
            string serverKey = "AAAAY8UVqoU:APA91bHD9ICFvT1CdO-gcHyo4p69tfQfXNJa_dM0Y5JyXrqzezUZt0cG-ax_DOCg-bvDspgUBOTxpRb2IXvOhyiE6o7RBYFMzkVJct65LniMec0LIo8rQF3pMUDybc4gNc8jlcgeAq1D";

            try
            {
                var result = "-1";
                var webAddr = "https://fcm.googleapis.com/fcm/send";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Method = "POST";
                //string json = "{\"to\": \"c7cOP-6Sn_4:APA91bEaj-PBS5c91p1FiPll08DTzpCZRf3RmOJcqvj4wWQqvB-6OTgrI3n_320lkL-d2rpPkNhtIeSSIX6zS8w287hQabHP8g6Yitv8YhtXAZaQTIz9D3emLyq7MN_GueDyG-qJWZNy\",\"data\": {\"message\": \"This is a Firebase Cloud Messaging Topic Message!\",}}";
                string json = JsonConvert.SerializeObject(request.data);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                   
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                // return result;
            }
            catch (Exception ex)
            {
                //  Response.Write(ex.Message);
            }
        }
        #endregion
    }
}
