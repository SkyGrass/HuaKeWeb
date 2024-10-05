using HuakeWeb.Utils;
using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI.WebControls;

namespace HuakeWeb
{
    public class Global : System.Web.HttpApplication
    {
        public static string errMsg = "";
        public static string configs = "";
        public static List<string> whiteActions = new List<string>() { "bind", "send-msg", "query-vendor" };
        protected void Application_Start(object sender, EventArgs e)
        {
            string connection = ConfigurationManager.AppSettings["conn"] ?? "";
            ZYSoft.DB.BLL.Common.SetConnString(connection);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpRequest request = ((HttpApplication)sender).Request;
            string path = request.Path;
            if (path.EndsWith("ashx"))
            {
                string action = request.QueryString["action"];
                if (!whiteActions.Contains(action))
                {
                    string session = Utils.Utils.GetCookie(request, "session", "");
                    if (string.IsNullOrEmpty(session) || !ZYSoft.DB.BLL.Common.Exist(string.Format(Const.SQL_USER_INFO, session)))
                    {
                        Response.ContentType = "application/json";
                        Response.AddHeader("Content-Type", "application/json;charset=UTF-8");
                        Response.Charset = "UTF-8";
                        Response.Write(AjaxResult.expired());
                        Response.End();
                    }
                }
            }
        }

    }
}