using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using TelerikMvcWebMail.Models;

namespace TelerikMvcWebMail.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        public  ActionResult Index(string ReturnUrl=null)
        {
          
                return View();           
        }

        
                
        public ActionResult ValidateUser(string UserEmail)
        {                      
            SignIn Model = new Models.SignIn();
            Model.UserName = UserEmail;
            SessionMangment.Users_.APIHostUrl = System.Configuration.ConfigurationManager.AppSettings["APIHostUrl"];
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/ValidateUser", RestSharp.Method.POST, Model);
            UserViewModel _User = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<UserViewModel>(Data);
            if (_User == null)
            {
                return RedirectToAction("Index", "Home", new { ReturnUrl = "UserNameInvalid" });
            }
            else
            {
                SessionMangment.Users_.FullName = _User.FirstName + " " + _User.LastName;
                SessionMangment.Users_.UserEmail = _User.Email;
                SessionMangment.Users_.UserId = _User.id.ToString();
                return RedirectToAction("Index", "File");
            }           
        }

        /// <summary>
        /// Send an OpenID Connect sign-in request.
        /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
        /// </summary>
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        /// <summary>
        /// Send an OpenID Connect sign-out request.
        /// </summary>
        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);
        }
        
    }
}