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
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(string ReturnUrl=null)
        {
            try
            {
                if (ReturnUrl == "UserNameInvalid")
                {
                   
                    ViewBag.LoginError = "You dont have permition to view this application";
                    return View();
                }          
                else if(ReturnUrl == "Loginagin")
                {
                    SignOut();
                    return View();
                }    
                else
                {
                    var claimsPrincipalCurrent = System.Security.Claims.ClaimsPrincipal.Current;
                    //You get the user’s first and last name below:
                    var Name = claimsPrincipalCurrent.FindFirst("name").Value;
                    // The 'preferred_username' claim can be used for showing the username
                    var Username = claimsPrincipalCurrent.FindFirst("preferred_username").Value;
                    // The subject claim can be used to uniquely identify the user across the web
                    var Subject = claimsPrincipalCurrent.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                    // TenantId is the unique Tenant Id - which represents an organization in Azure AD
                    var TenantId = claimsPrincipalCurrent.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;                   
                    return RedirectToAction("ValidateUser", "Home", new { UserEmail = Username });
                }
            }
            catch(Exception ex)
            {
                return View();
            }
            
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