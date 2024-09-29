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
                string code = Request.QueryString["code"] ?? "";
                string session = SafeConvert.SafeString(Request.Cookies.Get("session")?.Value, "");
                if (string.IsNullOrEmpty(code))
                {
                    if (!string.IsNullOrEmpty(session))
                    {
                        DataTable userSession = null;
                        try
                        {
                            userSession = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_QUERY_USER_SESSION, session));
                        }
                        catch (Exception)
                        {
                            Global.errMsg = "检查用户授权发生异常";
                            Response.Redirect(string.Format(@"sysError"), true);
                        }
                        if (userSession == null || userSession.Rows.Count <= 0)
                        {
                            Global.errMsg = "用户未授权或授权过期,请重新发起";
                            Response.Redirect(string.Format(@"sysError"), true);
                        }
                        else
                        {
                            Response.Redirect("home", true);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (new AppHelper(this).GetWxConfig(out config, out errMsg))
                            {
                                if (string.IsNullOrEmpty(config.RedirectUrl))
                                    config.RedirectUrl = Request.Url.AbsoluteUri;
                                string url = string.Format(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect", config.AppId, HttpUtility.HtmlEncode(config.RedirectUrl), Utils.Utils.GetRandomString());
                                Response.Redirect(url, true);
                            }
                            else
                            {
                                Response.Redirect(string.Format(@"sysError?reason={0}", HttpUtility.UrlEncode(errMsg)), true);
                            }
                        }
                        catch (Exception)
                        {
                            Global.errMsg = "查询系统授权参数发生异常\r\n请检查系统配置";
                            Response.Redirect(string.Format(@"sysError"), true);
                        }
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
                            Response.Redirect(string.Format(@"sysError"), true);
                        }
                    }
                    else
                    {
                        Response.Redirect(string.Format(@"sysError?reason={0}", HttpUtility.HtmlEncode(errMsg)), true);
                    }
                }
            }
        }
    }
}