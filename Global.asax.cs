using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace HuakeWeb
{
    public class Global : System.Web.HttpApplication
    {
        public static string errMsg = "";
        protected void Application_Start(object sender, EventArgs e)
        {
            string connection = ConfigurationManager.AppSettings["conn"] ?? "";
            ZYSoft.DB.BLL.Common.SetConnString(connection);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}