using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcWebMail
{
    public static class SessionMangment
    {
        public class Users_
        {
            public static string APIHostUrl
            {
                get { return Convert.ToString(HttpContext.Current.Session["APIHostUrl"]); }
                set { HttpContext.Current.Session["APIHostUrl"] = value; }
            }
            public static string FullName
            {
                get { return Convert.ToString(HttpContext.Current.Session["FullName"]); }
                set { HttpContext.Current.Session["FullName"] = value; }
            }
            public static string UserEmail
            {
                get { return Convert.ToString(HttpContext.Current.Session["UserEmail"]); }
                set { HttpContext.Current.Session["UserEmail"] = value; }
            }
            public static string UserId
            {
                get { return Convert.ToString(HttpContext.Current.Session["UserId"]); }
                set { HttpContext.Current.Session["UserId"] = value; }
            }
        }
    }
}
