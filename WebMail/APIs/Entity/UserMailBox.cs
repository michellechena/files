//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APIs.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserMailBox
    {
        public int UserId { get; set; }
        public int MailboxId { get; set; }
        public Nullable<bool> IsMainContact { get; set; }
        public Nullable<bool> IsDefoultMailBox { get; set; }
        public string PermitionLevel { get; set; }
    
        public virtual MailBox MailBox { get; set; }
    }
}
