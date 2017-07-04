//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bizkasa.Bizlunch.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class DB_TB_ORDERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DB_TB_ORDERS()
        {
            this.DB_TB_ORDER_DETAIL = new HashSet<DB_TB_ORDER_DETAIL>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LunchDate { get; set; }
        public Nullable<int> RestaurantId { get; set; }
        public int OwnerId { get; set; }
    
        public virtual DB_TB_ACCOUNTS DB_TB_ACCOUNTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_ORDER_DETAIL> DB_TB_ORDER_DETAIL { get; set; }
        public virtual DB_TB_RESTAURANT DB_TB_RESTAURANT { get; set; }
    }
}
