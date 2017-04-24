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
    
    public partial class DB_TB_RESTAURANT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DB_TB_RESTAURANT()
        {
            this.DB_TB_ACCOUNT_RESTAURANT = new HashSet<DB_TB_ACCOUNT_RESTAURANT>();
            this.DB_TB_ORDERS = new HashSet<DB_TB_ORDERS>();
        }
    
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string MenuUrl { get; set; }
        public string Phone { get; set; }
        public Nullable<bool> IsDelivery { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_ACCOUNT_RESTAURANT> DB_TB_ACCOUNT_RESTAURANT { get; set; }
        public virtual DB_TB_ACCOUNTS DB_TB_ACCOUNTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_ORDERS> DB_TB_ORDERS { get; set; }
    }
}
