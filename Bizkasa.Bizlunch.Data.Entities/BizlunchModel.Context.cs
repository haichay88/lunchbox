﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BizlunchEntities : DbContext
    {
        public BizlunchEntities()
            : base("name=BizlunchEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DB_TB_ORDER_DETAIL> DB_TB_ORDER_DETAIL { get; set; }
        public virtual DbSet<DB_TB_RESTAURANT> DB_TB_RESTAURANT { get; set; }
        public virtual DbSet<DB_TB_ACCOUNT_RESTAURANT> DB_TB_ACCOUNT_RESTAURANT { get; set; }
        public virtual DbSet<DB_TB_FRIENDSHIP> DB_TB_FRIENDSHIP { get; set; }
        public virtual DbSet<DB_TB_ACCOUNTS> DB_TB_ACCOUNTS { get; set; }
        public virtual DbSet<DB_TB_ORDERS> DB_TB_ORDERS { get; set; }
    }
}