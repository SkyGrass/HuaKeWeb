using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
        protected void Application_Start(object sender, EventArgs e)
        {
            string connection = ConfigurationManager.AppSettings["conn"] ?? "";
            ZYSoft.DB.BLL.Common.SetConnString(connection);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_OnError(object sender, EventArgs e)
        {
            try
            {
                string tracingFile = Server.MapPath("~/logs");
                if (!Directory.Exists(tracingFile))
                    Directory.CreateDirectory(tracingFile);
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                tracingFile = Path.Combine(tracingFile, fileName);
                if (tracingFile != string.Empty)
                {
                    FileInfo file = new FileInfo(tracingFile);
                    using (StreamWriter debugWriter = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                    {
                        debugWriter.WriteLine(DateTime.Now.ToString());
                        debugWriter.WriteLine(((HttpApplication)sender).Context.Error.Message);
                        debugWriter.WriteLine(((HttpApplication)sender).Context.Error.StackTrace);
                        debugWriter.WriteLine();
                        debugWriter.Flush();
                        debugWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}