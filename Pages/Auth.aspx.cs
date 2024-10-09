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
using System.Security.Cryptography;

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
                AppHelper.WriteLog(Request.QueryString.ToString());
                string code = Request.QueryString["code"] ?? "";
                string session = Utils.Utils.GetCookie(Request, "session", "");
                string target = Request.QueryString["target"] ?? "home";
                string id = Request.QueryString["id"] ?? "";
                if (ZYSoft.DB.BLL.Common.Exist(string.Format(Const.SQL_USER_INFO, session)))
                {
                    if (!string.IsNullOrEmpty(id))
                        Response.Redirect(target + "?id=" + id, false);
                    else
                        Response.Redirect(target, false);
                }
                else
                {
                    if (string.IsNullOrEmpty(code))
                    {
                        try
                        {
                            if (AppHelper.GetWxConfig(out config, out errMsg))
                            {
                                if (string.IsNullOrEmpty(config.RedirectUrl))
                                    config.RedirectUrl = Request.Url.AbsoluteUri;

                                config.RedirectUrl += ("?target=" + target);
                                if (!string.IsNullOrEmpty(id))
                                    config.RedirectUrl += ("&id=" + id);

                                string url = string.Format(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect", config.AppId, AppHelper.UrlEncode(config.RedirectUrl), Utils.Utils.GetRandomString());
                                AppHelper.WriteLog("重定向地址==>" + url);
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
                        if (AppHelper.GetAuthInfo(code, ref res, out errMsg))
                        {
                            string openId = res["openid"] ?? "";
                            if (!string.IsNullOrEmpty(openId))
                            {
                                if (!string.IsNullOrEmpty(id))
                                    Response.Redirect(string.Format(@"bind?target={0}&openId={1}&id={2}", target, openId, id)); //redirect auth
                                else
                                    Response.Redirect(string.Format(@"bind?target={0}&openId={1}", target, openId)); //redirect auth
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