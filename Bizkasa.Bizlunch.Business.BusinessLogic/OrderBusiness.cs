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
        List<OrderDTO> GetOrders();
        OrderViewDTO GetOrderBy(int orderid);
        bool AddOrUpdateOrder(OrderDTO dto);
        OrderViewDTO AddOrderDetail(OrderDTO dto);
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
        /// Add new Order
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool AddOrUpdateOrder(OrderDTO dto)
        {
            try
            {
                var m_orderRepository = UnitOfWork.Repository<DB_TB_ORDERS>();
                if (dto.Id > 0)
                {
                    //update
                    var m_orderdetailRepository = UnitOfWork.Repository<DB_TB_ORDER_DETAIL>();
                    if (dto.OrderDetails.Count > 0)
                    {
                        foreach (var item in dto.OrderDetails)
                        {
                            m_orderdetailRepository.Add(new DB_TB_ORDER_DETAIL() {
                                AccountId=item.AccountId,
                                MenuCost=item.MenuCost,
                                MenuItem=item.MenuItem,
                                OrderId=dto.Id,
                                CreatedDate=DateTime.Now
                            });
                        }
                    }
                }
                else
                {
                    DB_TB_ORDERS m_order = new DB_TB_ORDERS()
                    {
                     Title=dto.Title,   
                        CreatedDate = DateTime.Now,
                        LunchDate = dto.LunchDate,
                        OwnerId = WorkContext.UserContext.UserId,
                        RestaurantId = dto.RestaurantId,
                    };
                    foreach (var item in dto.OrderDetails)
                    {
                        DB_TB_ORDER_DETAIL m_orderDetail = new DB_TB_ORDER_DETAIL()
                        {
                            AccountId = item.AccountId,
                            CreatedDate = DateTime.Now
                        };
                        m_order.DB_TB_ORDER_DETAIL.Add(m_orderDetail);
                    }
                    m_order.DB_TB_ORDER_DETAIL.Add(new DB_TB_ORDER_DETAIL()
                    {
                        AccountId = WorkContext.UserContext.UserId,
                        CreatedDate = DateTime.Now
                    });
                    m_orderRepository.Add(m_order);
                    
                }

                UnitOfWork.Commit();
                return !this.HasError;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Add order detail
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public OrderViewDTO AddOrderDetail(OrderDTO dto)
        {
            if (!dto.OrderDetails.Any())
            {
                base.AddError("can not insert menu");
                return null;
            }
            var m_orderdetailRepository = UnitOfWork.Repository<DB_TB_ORDER_DETAIL>();
            if (dto.Id > 0)
            {
                //update
               
                var m_orderDetails = m_orderdetailRepository.GetQueryable().Where(a => a.AccountId == WorkContext.UserContext.UserId && a.OrderId == dto.Id).ToList();
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
                            AccountId = WorkContext.UserContext.UserId,
                            MenuCost = item.MenuCost,
                            MenuItem = item.MenuItem,
                            OrderId = dto.Id,
                            CreatedDate = DateTime.Now
                        });
                    }
                   
                }

                UnitOfWork.Commit();
                
            }
            var result = m_orderdetailRepository.GetQueryable().Where(a => a.AccountId == WorkContext.UserContext.UserId && a.OrderId == dto.Id)
                    .Select(a => new OrderDetailDTO()
                    {
                        AccountId = a.AccountId,
                        CreatedDate = a.CreatedDate,
                        MenuCost = a.MenuCost,
                        MenuItem = a.MenuItem,
                        OrderId = a.OrderId,
                        Id = a.Id
                    }).ToList();
            return GetOrderBy(dto.Id);
        }
        public List<OrderDTO> GetOrders()
        {
            var m_orderRepository = UnitOfWork.Repository<DB_TB_ORDERS>().GetQueryable();
            var orders = m_orderRepository.Where(a => a.DB_TB_ORDER_DETAIL.Any(b => b.AccountId == WorkContext.UserContext.UserId))
                   .Select(a => new OrderDTO() {
                       Title=a.Title,
                       Id=a.Id,
                       CreatedDate=a.CreatedDate,
                       LunchDate=a.LunchDate.Value,
                       RestaurantId=a.RestaurantId
                       //OrderDetails=a.DB_TB_ORDER_DETAIL.Where(e=>e.MenuCost>0).Select(b=>new OrderDetailDTO() {
                       //    AccountId=b.AccountId,
                       //    CreatedDate=b.CreatedDate,
                       //    Id=b.Id,
                       //    MenuCost=b.MenuCost,
                       //    MenuItem=b.MenuItem
                       //}).ToList()
                   }).ToList();
          

            return orders;
        }

        /// <summary>
        /// Get order by Id
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public OrderViewDTO GetOrderBy(int orderid)
        {
            var m_orderRepository = UnitOfWork.Repository<DB_TB_ORDERS>();
            var m_order = m_orderRepository.GetQueryable().Where(a => a.Id == orderid).Select(a => new OrderViewDTO()
            {
                Id = a.Id,
                Title=a.Title,
                CreatedDate = a.CreatedDate,
                LunchDate = a.LunchDate.Value,
                RestaurantId = a.RestaurantId,
                TotalAmount=a.DB_TB_ORDER_DETAIL.Where(x=>x.MenuCost.HasValue).Sum(b=>b.MenuCost).Value,
                RestaurantName = a.DB_TB_RESTAURANT.Name,
                OwnerName=a.DB_TB_ACCOUNTS.ACC_FIRSTNAME +" " +a.DB_TB_ACCOUNTS.ACC_LASTNAME,
                MenuURL = a.DB_TB_RESTAURANT.MenuUrl,
                RestaurantPhone = a.DB_TB_RESTAURANT.Phone,                
                OrderDetails = a.DB_TB_ORDER_DETAIL.Select(b => new OrderDetailDTO()
                {
                    OwnerName=b.DB_TB_ACCOUNTS.ACC_FIRSTNAME + " " +b.DB_TB_ACCOUNTS.ACC_LASTNAME,
                    AccountId = b.AccountId,
                    CreatedDate = b.CreatedDate,
                    Id = b.Id,
                    MenuCost = b.MenuCost,
                    MenuItem = b.MenuItem,
                   
                }).ToList()
            }).FirstOrDefault();

            // lấy những thành viên có trong order
            var accounts = m_order.OrderDetails.Select(a => a.AccountId).Distinct().ToList();
            
            // Lấy danh sách của thành viên đang đăng nhập (để có thể chỉnh sửa)
            m_order.OrderDetailsCanEdit = m_order.OrderDetails.Where(a => a.AccountId == WorkContext.UserContext.UserId).Select(a => new OrderDetailDTO() {
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
                        row.IsOwner = m_orderdetail.AccountId == WorkContext.UserContext.UserId;
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
