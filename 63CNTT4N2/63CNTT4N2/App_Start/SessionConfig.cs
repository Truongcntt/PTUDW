using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _63CNTT4N2.App_Start
{
    public static class SessionConfig
    {
        public static void SetUser(Users user)
        {
            HttpContext.Current.Session["user"] = user;
        }
        public static Users GetUser()
        {
            return (Users)HttpContext.Current.Session["user"];
        }

    }
}