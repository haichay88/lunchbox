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
    
    public partial class DB_TB_ACCOUNTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DB_TB_ACCOUNTS()
        {
            this.DB_TB_ACCOUNT_RESTAURANT = new HashSet<DB_TB_ACCOUNT_RESTAURANT>();
            this.DB_TB_FRIENDSHIP = new HashSet<DB_TB_FRIENDSHIP>();
            this.DB_TB_FRIENDSHIP1 = new HashSet<DB_TB_FRIENDSHIP>();
            this.DB_TB_ORDER_DETAIL = new HashSet<DB_TB_ORDER_DETAIL>();
            this.DB_TB_RESTAURANT = new HashSet<DB_TB_RESTAURANT>();
            this.DB_TB_ORDERS = new HashSet<DB_TB_ORDERS>();
        }
    
        public int ACC_SYS_ID { get; set; }
        public string ACC_EMAIL { get; set; }
        public string ACC_PASSWORD { get; set; }
        public string ACC_FIRSTNAME { get; set; }
        public string ACC_LASTNAME { get; set; }
        public string ACC_MIDDLENAME { get; set; }
        public Nullable<System.DateTime> ACC_LASTLOGIN_DATE { get; set; }
        public Nullable<int> ACC_OWNER_ID { get; set; }
        public string ACC_SOURCE_ID { get; set; }
        public string ACC_TOKEN { get; set; }
        public string ACC_RESGISTRANTION_ID { get; set; }
        public bool ACC_IS_ACTIVED { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_ACCOUNT_RESTAURANT> DB_TB_ACCOUNT_RESTAURANT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_FRIENDSHIP> DB_TB_FRIENDSHIP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_FRIENDSHIP> DB_TB_FRIENDSHIP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_ORDER_DETAIL> DB_TB_ORDER_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_RESTAURANT> DB_TB_RESTAURANT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_TB_ORDERS> DB_TB_ORDERS { get; set; }
    }
}
