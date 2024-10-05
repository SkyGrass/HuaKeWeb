using HuakeWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HuakeWeb.Pages
{
    public partial class Detail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //if (new AppHelper(this).GetWxConfig(out ZYSoftConfig config, out string errMsg))
                //{
                //    if (new AppHelper(this).GetWxTicket(out string jsapiTicket, out errMsg))
                //    {
                //        Dictionary<string, string> configs = new AppHelper(this).GenerateSignature(config.AppId, jsapiTicket, 
                //            Request.Url.AbsoluteUri.ToString());
                //        Global.configs = JsonConvert.SerializeObject(configs);
                //    }
                //    else
                //    {
                //        Global.errMsg = errMsg;
                //        Response.Redirect("sysError", false);
                //    }
                //}
            }
        }
    }
}