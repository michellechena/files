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
    
    public partial class MailBox
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MailBox()
        {
            this.Folders = new HashSet<Folder>();
            this.UserMailBoxes = new HashSet<UserMailBox>();
        }
    
        public int id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public byte StatusId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Folder> Folders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserMailBox> UserMailBoxes { get; set; }
    }
}