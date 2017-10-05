using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TelerikMvcWebMail.Filter
{
    public class SessionExpire: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {           
            if (HttpContext.Current.Session["UserId"] == null)
            {               
                
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{{ "controller", "SignIn" },
                                          { "action", "Index" },{ "ReturnUrl", "Sessionexpire" }

                                         });
            }
            
            base.OnActionExecuting(filterContext);
        }

    }
}