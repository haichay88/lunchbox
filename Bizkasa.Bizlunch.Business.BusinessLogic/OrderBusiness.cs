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
    public interface IOrderBusiness
    {
        List<OrderDTO> GetOrders(SearchDTO request);
        OrderViewDTO GetOrderBy(SearchDTO request);
      
        OrderViewDTO AddOrderDetail(OrderDTO dto);
        bool AddInvite(InviteDTO request);
        bool AddMoreFriend(InviteMoreFriendDTO request);
        bool CheckExistFriendShip(int accountId, int friendId);
        DB_TB_FRIENDSHIP BuildRowFriendShip(int accountId, int friendId);
    }
    public class OrderBusiness:BusinessBase, IOrderBusiness
    {
        #region Contructors
        public OrderBusiness() { }
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
       

        /// <summary>
        /// Add order detail
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public OrderViewDTO AddOrderDetail(OrderDTO dto)
        {
            if (dto.Context == null)
            {
                base.AddError("Authenticate failed !");
                return null;
            }

            if (!dto.OrderDetails.Any())
            {
                base.AddError("can not insert menu");
                return null;
            }
            var m_orderdetailRepository = UnitOfWork.Repository<DB_TB_ORDER_DETAIL>();
            if (dto.Id > 0)
            {
                //update
               
                var m_orderDetails = m_orderdetailRepository.GetQueryable().Where(a => a.AccountId == dto.Context.Id && a.OrderId == dto.Id).ToList();
                if (m_orderDetails.Any())
                {
                    foreach (var item in m_orderDetails)
                    {
                        m_orderdetailRepository.Delete(item);
                    }
                }
                foreach (var item in dto.OrderDetails)
                {
                    if (!string.IsNullOrEmpty(item.MenuItem))
                    {
                        m_orderdetailRepository.Add(new DB_TB_ORDER_DETAIL()
                        {
                            AccountId = dto.Context.Id,
                            MenuCost = item.MenuCost,
                            MenuItem = item.MenuItem,
                            OrderId = dto.Id,
                            CreatedDate = DateTime.Now
                        });
                    }
                   
                }

                UnitOfWork.Commit();
                
            }
            var result = m_orderdetailRepository.GetQueryable().Where(a => a.AccountId == dto.Context.Id && a.OrderId == dto.Id)
                    .Select(a => new OrderDetailDTO()
                    {
                        AccountId = a.AccountId,
                        CreatedDate = a.CreatedDate,
                        MenuCost = a.MenuCost,
                        MenuItem = a.MenuItem,
                        OrderId = a.OrderId,
                        Id = a.Id
                    }).ToList();
            SearchDTO search = new SearchDTO()
            {
                Id = dto.Id,
                Token = dto.Token
            };
            return GetOrderBy(search);
        }
        public List<OrderDTO> GetOrders(SearchDTO request)
        {
            if (request.Context == null)
            {
                base.AddError("Authenticate failed !");
                return null;
            }

            var m_orderRepository = UnitOfWork.Repository<DB_TB_ORDERS>().GetQueryable();
            var orders = m_orderRepository.Where(a => a.DB_TB_ORDER_DETAIL.Any(b => b.AccountId == request.Context.Id))
                   .Select(a => new OrderDTO() {
                       Title=a.Title,
                       Id=a.Id,
                       CreatedDate=a.CreatedDate,
                       Description=a.Description,
                       LunchDate=a.LunchDate.Value,
                       RestaurantId=a.RestaurantId,
                       RestaurantName = a.RestaurantId.HasValue? a.DB_TB_RESTAURANT.Name:string.Empty,
                       RestaurantAddress= a.RestaurantId.HasValue ? a.DB_TB_RESTAURANT.Address:string.Empty
                      
                   }).OrderByDescending(a=>a.LunchDate).Skip(request.Skip).Take(request.Take).ToList();
          

            return orders;
        }
        public void AddFriendShip(int orderid) 
        {
          
         
            var m_friendshipRepository = UnitOfWork.Repository<DB_TB_FRIENDSHIP>();
            var m_accountInOrder = UnitOfWork.Repository<DB_TB_ORDER_DETAIL>().GetQueryable().Where(a=>a.OrderId== orderid).Select(a=>a.AccountId.Value).Distinct().ToList();

            foreach (var item in m_accountInOrder)
            {
                var friendExcept = m_accountInOrder.Where(a =>a!= item).ToList();
              
               
                // add for other friend
                foreach (var other in friendExcept)
                {
                    if (!CheckExistFriendShip(item, other))
                    {
                        m_friendshipRepository.Add(BuildRowFriendShip(item, other));
                        
                    }
                    if (!CheckExistFriendShip(other, item)) { m_friendshipRepository.Add(BuildRowFriendShip(other, item)); }
                }
                UnitOfWork.Commit();
            }
            
        }
        public void addRestaurantAccount(int orderId)
        {
            var m_retaurantshipRepository = UnitOfWork.Repository<DB_TB_ACCOUNT_RESTAURANT>();
            var m_orderdetailRepository = UnitOfWork.Repository<DB_TB_ORDER_DETAIL>().GetQueryable();
            var m_accountInOrder = m_orderdetailRepository.Where(a => a.OrderId == orderId).Select(a => a.AccountId.Value).Distinct().ToList();
            var restaurantId = UnitOfWork.Repository<DB_TB_ORDERS>().GetQueryable().Where(a => a.Id == orderId).Select(a => a.RestaurantId).FirstOrDefault();
            if (!restaurantId.HasValue) return;
            foreach (var item in m_accountInOrder)
            {

                bool existRestaurant = m_retaurantshipRepository.GetQueryable().Where(a => a.RestaurantId == restaurantId && a.AccountId == item).FirstOrDefault() != null;
                if (!existRestaurant) {
                    var accountRestaurant = new DB_TB_ACCOUNT_RESTAURANT()
                    {
                        AccountId=item,
                        RestaurantId=restaurantId.Value,
                        CreatedDate=DateTime.Now
                    };
                    m_retaurantshipRepository.Add(accountRestaurant);
                }
                // add for other friend
               
               
            }
            UnitOfWork.Commit();
        }
        public bool CheckExistFriendShip(int accountId, int friendId)
        {
            return UnitOfWork.Repository<DB_TB_FRIENDSHIP>().GetQueryable().Any(a => a.AccountId == accountId && a.FriendId == friendId);
        }
        public DB_TB_FRIENDSHIP BuildRowFriendShip(int accountId, int friendId)
        {
            return new DB_TB_FRIENDSHIP() {
                AccountId = accountId,
                FriendId = friendId,
                CreatedDate = DateTime.Now
            };
        }
        public bool AddInvite(InviteDTO request)
        {
            if (request.Context == null)
            {
                base.AddError("Authenticate failed !");
                return false;
            }
            if(request.Friends==null)
            {
                base.AddError("Data invaild!");
                return false;
            }
           
            var m_orderRepository = UnitOfWork.Repository<DB_TB_ORDERS>();
            var m_friendRepository = UnitOfWork.Repository<DB_TB_ACCOUNTS>();
            var m_orderDetailRepository = UnitOfWork.Repository<DB_TB_ORDER_DETAIL>();
            var m_orderAccountRepository = UnitOfWork.Repository<DB_TB_INVITE_ACCOUNT>();

            var m_invite = new DB_TB_ORDERS()
            {
                Title=request.Title,
                LunchDate=request.LunchDate,
                OwnerId=request.Context.Id,
                RestaurantId=request.PlaceId,
                CreatedDate=DateTime.Now,
               Description=request.Description
            };
            if (!string.IsNullOrEmpty( request.Place.Name ) && string.IsNullOrEmpty(request.Place.Address))
            {
                var row = new DB_TB_RESTAURANT()
                {
                    Address = request.Place.Address,
                    Name = request.Place.Name,
                    Latitude = request.Place.Latitude,
                    Longitude = request.Place.Longitude,
                    MenuUrl = request.Place.MenuUrl,
                    OwnerId = request.Context.Id,
                    CreatedDate = DateTime.Now
                };
                UnitOfWork.Repository<DB_TB_RESTAURANT>().Add(row);
                m_invite.DB_TB_RESTAURANT = row;
            }
            // add current account to order detail
            var m_currentuserDetail = new DB_TB_ORDER_DETAIL()
            {
                DB_TB_ORDERS = m_invite,
                AccountId=request.Context.Id,
                CreatedDate=DateTime.Now
            };
            m_orderDetailRepository.Add(m_currentuserDetail);
           

            // add friend to order detail
            foreach (var item in request.Friends)
            {

                var rowAccountInvite = new DB_TB_INVITE_ACCOUNT()
                {
                    
                    IsConfirmed = false,
                    IsSent = false,
                    DB_TB_ORDERS = m_invite
                };

                var m_inviteDetail = new DB_TB_ORDER_DETAIL()
                {
                    DB_TB_ORDERS = m_invite,
                  
                };
                // FRIEND SHIP
              
                if (item.Id <= 0)
                {
                    // add friend ship

                    var m_account = new DB_TB_ACCOUNTS()
                    {
                        ACC_EMAIL=item.Email,
                        ACC_IS_ACTIVED=false,
                       ACC_FIRSTNAME=item.FirstName
                    };
                    // check exist by email
                    var exist = m_friendRepository.GetQueryable().Where(a => a.ACC_EMAIL == item.Email).FirstOrDefault();
                    if (exist == null)
                    {
                        m_friendRepository.Add(m_account);
                        m_inviteDetail.DB_TB_ACCOUNTS = m_account;
                        rowAccountInvite.DB_TB_ACCOUNTS = m_account;
                    }
                    else
                    {
                        m_inviteDetail.AccountId = exist.ACC_SYS_ID;
                        rowAccountInvite.AccountId = exist.ACC_SYS_ID;
                    }
                    
                }
                else
                {
                    m_inviteDetail.AccountId = item.Id;
                    rowAccountInvite.AccountId = item.Id;
                }
                m_inviteDetail.CreatedDate = DateTime.Now;
                m_orderDetailRepository.Add(m_inviteDetail);
                m_orderAccountRepository.Add(rowAccountInvite);
            }
            UnitOfWork.Commit();


            // update friend ship
            AddFriendShip(m_invite.Id);

            // update restaurant per account
            addRestaurantAccount(m_invite.Id);

            // send email 
            SendMessageInvite(m_invite.Id);


            return !this.HasError;
        }
        /// <summary>
        /// gui notify hoac email moi tham gia 
        /// </summary>
        /// <param name="inviteId"></param>
        public void SendMessageInvite(int inviteId)
        {

            var invite = UnitOfWork.Repository<DB_TB_ORDERS>().GetQueryable()
                .Where(a => a.Id == inviteId && a.DB_TB_INVITE_ACCOUNT.Any(c=>!c.IsSent))
                .Select(a => new {
                    Friends=a.DB_TB_INVITE_ACCOUNT.Select(b=> new {
                        InviteAccountId=b.Id,
                        Name=b.DB_TB_ACCOUNTS.ACC_FIRSTNAME,
                        Email = b.DB_TB_ACCOUNTS.ACC_EMAIL,
                        DeviceKey = b.DB_TB_ACCOUNTS.ACC_RESGISTRANTION_ID,
                        IsEmail = b.DB_TB_ACCOUNTS.ACC_RESGISTRANTION_ID==null,
                        IsSent = b.IsSent,
                        IsConfirm = b.IsConfirmed,
                    }).ToList(),                   
                    Name=a.DB_TB_ACCOUNTS.ACC_FIRSTNAME,
                    Title=a.Title,
                    Place=a.DB_TB_RESTAURANT!=null? a.DB_TB_RESTAURANT.Name:string.Empty,
                    Time=a.LunchDate.Value,
                    Owner=new
                    {
                        Name=a.DB_TB_ACCOUNTS.ACC_FIRSTNAME
                    }
                })
                .FirstOrDefault();

            if (invite == null) return;
            if (!invite.Friends.Any()) return;
            // send email or notify foreach friend 
            var m_accountInviteRepository = UnitOfWork.Repository<DB_TB_INVITE_ACCOUNT>();
            foreach (var item in invite.Friends)
            {
                if(!item.IsSent)
                {
                    if (item.IsEmail)
                    {
                        // send email
                        try
                        {
                            if (ConfigKey.IS_DEBUG != "DEBUG")
                                IoC.Get<IAccountBusiness>().SendOneEmailInvite(new InviteEmailDTO()
                                {
                                    LunchDate = invite.Time.ToStringVN(),
                                    Place = invite.Place,
                                    ReceiverEmail = item.Email,
                                    ReceiverName = item.Name,
                                    Sender = invite.Owner.Name,
                                    Title = invite.Title
                                });
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                        
                       
                    }
                    else
                    {
                        // push notify
                        if (ConfigKey.IS_DEBUG != "DEBUG")
                            try
                            {
                                var ac = new List<ActionNotify>();
                                ac.Add(new ActionNotify());

                                IoC.Get<IAccountBusiness>().PushMessage(new NotificationDTO()
                                {
                                    data = new NotificationItem()
                                    {
                                        id = inviteId,
                                        message = invite.Title,
                                        title = string.Format(" New invite started by {0}", invite.Owner.Name),
                                        actions =ac

                                    },
                                    to = item.DeviceKey
                                });
                            }
                            catch (Exception)
                            {

                                continue;
                            }
                           
                    }

                    // update sent when send email success
                    var row = m_accountInviteRepository.Get(a => a.Id == item.InviteAccountId);
                    row.IsSent = true;
                    m_accountInviteRepository.Update(row);
                    UnitOfWork.Commit();
                }
            }
        }

        public bool AddMoreFriend(InviteMoreFriendDTO request)
        {
            if (request.Context == null)
            {
                base.AddError("Authenticate failed !");
                return false;
            }
            if (request.Friends == null)
            {
                base.AddError("Data invaild!");
                return false;
            }
            var m_orderDetailRepository = UnitOfWork.Repository<DB_TB_ORDER_DETAIL>();
            var m_orderDetails = m_orderDetailRepository.GetQueryable().Where(a => a.OrderId == request.OrderId).ToList();
            var m_orderAccountRepository = UnitOfWork.Repository<DB_TB_INVITE_ACCOUNT>();
            if (!m_orderDetails.Any())
            {
                base.AddError("Data invaild!");
                return false;
            }
          
            foreach (var item in request.Friends)
            {
                if (!string.IsNullOrEmpty(item.Email))
                {
                    if (item.Id>0)
                    {
                        bool checkExist= m_orderDetails.Where(a => a.AccountId == item.Id).FirstOrDefault()!=null;

                        if (!checkExist)
                        {
                            var m_friendDetail = new DB_TB_ORDER_DETAIL()
                            {
                               OrderId=request.OrderId,
                                AccountId = item.Id,
                                CreatedDate = DateTime.Now
                            };
                            m_orderDetailRepository.Add(m_friendDetail);

                            // add account in invite
                            m_orderAccountRepository.Add(new DB_TB_INVITE_ACCOUNT()
                            {
                                AccountId = item.Id,
                                IsConfirmed = false,
                                IsSent = false,
                                InviteId = request.OrderId
                            });

                        }
                    }
                   
                }
                
            }
            UnitOfWork.Commit();
            // update  friend ship 
            AddFriendShip(request.OrderId);

            // update restaurant per account
            addRestaurantAccount(request.OrderId);
            // send email 
            SendMessageInvite(request.OrderId);
            return !this.HasError;
        }
        /// <summary>
        /// Get order by Id
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public OrderViewDTO GetOrderBy(SearchDTO request)
        {
            if (request.Context == null)
            {
                base.AddError("Authenticate failed !");
                return null;
            }
            var m_orderRepository = UnitOfWork.Repository<DB_TB_ORDERS>();
            var m_order = m_orderRepository.GetQueryable().Where(a => a.Id == request.Id).Select(a => new OrderViewDTO()
            {
                Id = a.Id,
                Title=a.Title,
                Description=a.Description,
                CreatedDate = a.CreatedDate,
                LunchDate = a.LunchDate.Value,
                RestaurantId = a.RestaurantId,
                TotalAmount=a.DB_TB_ORDER_DETAIL.Sum(b=>b.MenuCost),
                RestaurantName = a.DB_TB_RESTAURANT.Name,
                RestaurantAddress=a.DB_TB_RESTAURANT.Address,
                OwnerName=a.DB_TB_ACCOUNTS.ACC_FIRSTNAME +" " +a.DB_TB_ACCOUNTS.ACC_LASTNAME,
                MenuURL = a.DB_TB_RESTAURANT.MenuUrl,
                RestaurantPhone = a.DB_TB_RESTAURANT.Phone,                
                OrderDetails = a.DB_TB_ORDER_DETAIL.Select(b => new OrderDetailDTO()
                {
                    OwnerName=b.DB_TB_ACCOUNTS.ACC_FIRSTNAME + " " +b.DB_TB_ACCOUNTS.ACC_LASTNAME,
                    AccountId = b.AccountId,
                    CreatedDate = b.CreatedDate,
                    Id = b.Id,
                    MenuCost = b.MenuCost.HasValue?b.MenuCost:0,
                    MenuItem = b.MenuItem,
                   
                }).ToList()
            }).FirstOrDefault();

            // lấy những thành viên có trong order
            var accounts = m_order.OrderDetails.Select(a => a.AccountId).Distinct().ToList();
            
            // Lấy danh sách của thành viên đang đăng nhập (để có thể chỉnh sửa)
            m_order.OrderDetailsCanEdit = m_order.OrderDetails.Where(a => a.AccountId == request.Context.Id && !string.IsNullOrWhiteSpace(a.MenuItem)).Select(a => new OrderDetailDTO() {
                AccountId=a.AccountId,
                CreatedDate=a.CreatedDate,
                Id=a.Id,
                MenuCost=a.MenuCost,
                MenuItem=a.MenuItem,
                OrderId=a.OrderId
            }).ToList();

            // danh sách rút gọn của tất cả các thành viên

            foreach (var item in accounts)
            {
              var m_orderDetails=  m_order.OrderDetails.Where(a => a.AccountId==item).ToList();
                var row = new OrderDetailViewDTO();
                // chưa tạo order ( tạo menu )
                row.IsNotOrderYet = m_orderDetails.Count(a => !string.IsNullOrWhiteSpace(a.MenuItem))<=0;
                if (!row.IsNotOrderYet)
                {
                    foreach (var m_orderdetail in m_orderDetails)
                    {
                        row.MenuItemSummary = string.IsNullOrEmpty(row.MenuItemSummary) ? m_orderdetail.MenuItem : row.MenuItemSummary + ", " + m_orderdetail.MenuItem;
                        row.MenuCostTotal = m_orderdetail.MenuCost.HasValue ? row.MenuCostTotal + m_orderdetail.MenuCost.Value : row.MenuCostTotal;
                        row.IsOwner = m_orderdetail.AccountId == request.Context.Id;
                        row.OwnerName = m_orderdetail.OwnerName;
                    }
                    m_order.OrderDetailsReadOnly.Add(row);
                }
                else
                {
                    m_order.OrderDetailsNotYet.Add(new OrderDetailViewDTO() { OwnerName = m_orderDetails.Select(a => a.OwnerName).FirstOrDefault() });
                }


            }
            return m_order;
        }
        #endregion
    }
}
