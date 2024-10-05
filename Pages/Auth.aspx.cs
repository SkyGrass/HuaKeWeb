using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using HuakeWeb.Utils;
using HuakeWeb.Models;
using System.Configuration;
using System.Web.Hosting;

namespace HuakeWeb.Pages
{
    public partial class Auth : System.Web.UI.Page
    {
        private string errMsg = "";
        private ZYSoftConfig config;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                new AppHelper(this).WriteLog(Request.QueryString.ToString());
                string code = Request.QueryString["code"] ?? "";
                string session = Utils.Utils.GetCookie(Request, "session", "");
                if (ZYSoft.DB.BLL.Common.Exist(string.Format(Const.SQL_USER_INFO, session)))
                {
                    Response.Redirect("home", false);
                }
                else
                {
                    if (string.IsNullOrEmpty(code))
                    {
                        try
                        {
                            if (new AppHelper(this).GetWxConfig(out config, out errMsg))
                            {
                                if (string.IsNullOrEmpty(config.RedirectUrl))
                                    config.RedirectUrl = Request.Url.AbsoluteUri;
                                string url = string.Format(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect", config.AppId, HttpUtility.HtmlEncode(config.RedirectUrl), Utils.Utils.GetRandomString());
                                Response.Redirect(url, false);
                            }
                            else
                            {
                                Global.errMsg = errMsg;
                                Response.Redirect("sysError", false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.errMsg = "查询系统授权参数发生异常\r\n请检查系统配置";
                            Response.Redirect(string.Format(@"sysError"), true);
                        }
                    }
                    else
                    {
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        if (new AppHelper(this).GetAuthInfo(code, ref res, out errMsg))
                        {
                            string openId = res["openid"] ?? "";
                            if (!string.IsNullOrEmpty(openId))
                            {
                                Response.Redirect(string.Format(@"bind?openId={0}", openId)); //redirect auth
                            }
                            else
                            {
                                Global.errMsg = "系统似乎开小差了,请关闭后打开!";
                                Response.Redirect("sysError", true);
                            }
                        }
                        else
                        {
                            Global.errMsg = errMsg;
                            Response.Redirect("sysError", false);
                        }
                    }
                }
            }
        }
    }
}