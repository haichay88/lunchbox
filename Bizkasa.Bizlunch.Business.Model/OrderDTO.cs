using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bizkasa.Bizlunch.Business.Utils;

namespace Bizkasa.Bizlunch.Business.Model
{
   public class OrderDTO:BaseRequest
    {
        public OrderDTO()
        {
            this.OrderDetails = new List<OrderDetailDTO>();
        }

        public int Id { get; set; }
        
        public string Title { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public System.DateTime LunchDate { get; set; }
        public string RestaurantName { get; set; }
        public string LunchDateText
        {
            get
            {
                return this.LunchDate.ToStringDateVN();
            }
        }

        public string CreatedTimeText
        {
            get
            {
                return CreatedDate.HasValue ? CreatedDate.Value.ToStringVN() : string.Empty;
            }
        }
       
        public int RestaurantId { get; set; }
       
        //  public virtual Restaurant Restaurant { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> AccountId { get; set; }
        public string OwnerName { get; set; }
        public string MenuItem { get; set; }
        public Nullable<decimal> MenuCost { get; set; }
        public System.DateTime CreatedDate { get; set; }

    }
    public class OrderViewDTO : OrderDTO {
        public OrderViewDTO()
        {
            OrderDetailsCanEdit = new List<OrderDetailDTO>();
            OrderDetailsReadOnly = new List<OrderDetailViewDTO>();
            OrderDetailsNotYet = new List<OrderDetailViewDTO>();
        }
        public decimal TotalAmount { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
       
        public string MenuURL { get; set; }
        public string RestaurantPhone { get; set; }
        public List<OrderDetailDTO> OrderDetailsCanEdit { get; set; }
        public List<OrderDetailViewDTO> OrderDetailsReadOnly { get; set; }
        public List<OrderDetailViewDTO> OrderDetailsNotYet { get; set; }
    }

    public class OrderDetailViewDTO 
    {
        public string MenuItemSummary { get; set; }
        public decimal MenuCostTotal { get; set; }
        public bool IsNotOrderYet { get; set; }
        public bool IsOwner { get; set; }
        public string OwnerName { get; set; }
    }

    public class InviteDTO :BaseRequest
    {
        public string Title { get; set; }
        public int PlaceId { get; set; }
        public System.DateTime LunchDate { get; set; }
        public List<FriendInInviteDTO> Friends { get; set; }

    }
    public class FriendInInviteDTO
    {
        public int FriendId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    }



}
