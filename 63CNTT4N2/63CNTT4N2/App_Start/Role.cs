using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace _63CNTT4N2.App_Start
{
    public class Role : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filtercontext)
        {
            var user = SessionConfig.GetUser();
            if (user == null)
            {
                filtercontext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new{controller = "Admin",action = "Dangnhap",area = "Admin"}));
                return;
            }
            return;
        }
    }
}