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
    
    public partial class DB_TB_INVITE_ACCOUNT
    {
        public int Id { get; set; }
        public int InviteId { get; set; }
        public int AccountId { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSent { get; set; }
    
        public virtual DB_TB_ACCOUNTS DB_TB_ACCOUNTS { get; set; }
        public virtual DB_TB_ORDERS DB_TB_ORDERS { get; set; }
    }
}
