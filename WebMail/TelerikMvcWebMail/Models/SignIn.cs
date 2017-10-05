using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TelerikMvcWebMail.Models
{
    public class SignIn
    {
        [Required(ErrorMessage = "User Name Required")]
        public string UserName { get; set; }
        
    }

    public partial class UserViewModel
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }
}