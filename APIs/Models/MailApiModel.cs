using APIs.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIs.Models
{
    public class UserApIModel
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
 
    }

    public class SignIn
    {
        
        public string UserName { get; set; }
       
        public string Password { get; set; }
    }
    public class FilesApiModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public byte TypeId { get; set; }
        public string FolderId { get; set; }
        public byte StatusId { get; set; }
        public bool IsValid { get; set; }
        public string PermitionLevel { get; set; }
    }
    public class UserMailBoxApiModel
    {
        public int UserId { get; set; }
        public int MailboxId { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public Nullable<bool> IsMainContact { get; set; }
        public Nullable<bool> IsDefoultMailBox { get; set; }
        public string PermitionLevel { get; set; }
      

    }

    public class FolderApiModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MailBoxId { get; set; }
        public string MailBoxName { get; set; }
        public string TypeId { get; set; }
        public string StatusId { get; set; }   
    }

    public class Folders
    {
        public string text { get; set; }
        public string value { get; set; }
        public string number { get; set; }
        public string MailBoxName { get; set; }
      
       
    }
}