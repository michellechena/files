using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelerikMvcWebMail.Models
{

    public class FilesViewModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        [Required(ErrorMessage = "File Name Required")]
        public string Name { get; set; }
        public byte TypeId { get; set; }
        public string FolderId { get; set; }
        public byte StatusId { get; set; }
        public bool IsValid { get; set; }
        public string PermitionLevel { get; set; }
        public string RequestFrom { get; set; }

    }

        
    public class FoldersViewmodel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mail Box Name Required")]
        public int MailBoxId { get; set; }
        public string MailBoxName { get; set; }

        [Required(ErrorMessage = "Folder type Required")]
        public byte ?TypeId { get; set; }
        public byte? StatusId { get; set; }
    }

    public class Folders
    {
        public string text { get; set; }
        public string value { get; set; }
        public string number { get; set; }
        public string MailBox { get; set; }
        public string Active { get; set; }
        public string Disable { get; set; }
        public string Owner { get; set; }
    }
}