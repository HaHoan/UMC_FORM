using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UMC_FORM.Models;

namespace UMC_FORM.Authentication
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            var userId = Convert.ToString(httpContext.Session["userId"]);
            if (!string.IsNullOrEmpty(userId))
                using (var context = new DataContext())
                {
                    var userRole = (from u in context.Form_User
                                    join r in context.Form_Roles on u.ROLE_ID equals r.ID
                                    where u.CODE == userId
                                    select new
                                    {
                                        r.NAME
                                    }).FirstOrDefault();
                    foreach (var role in allowedroles)
                    {
                        if (role.ToUpper() == userRole.NAME.ToUpper()) return true;
                    }
                }


            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "Index" }
               });
        }
    }
}
