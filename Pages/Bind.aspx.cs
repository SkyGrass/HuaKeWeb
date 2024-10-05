using HuakeWeb.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HuakeWeb.Pages
{
    public partial class Bind : System.Web.UI.Page
    {
        public int IsBind = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string openId = Request.QueryString["openId"] ?? "";
                if (!string.IsNullOrEmpty(openId))
                {
                    DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_SELECT_VENDOR_BY_OPENID, openId));
                    if (null != dt && dt.Rows.Count > 0)
                    {
                        IsBind = 1;
                        string cVenCode = SafeConvert.SafeString(dt.Rows[0]["cVenCode"], "");

                        string session = ZYSoft.DB.BLL.Common.ExecuteScalar(string.Format(Const.SQL_USER_SESSION_BY_CODE, cVenCode));
                        if (!string.IsNullOrEmpty(session))
                        {
                            Thread.Sleep(300);
                            Response.Cookies.Add(new HttpCookie("session", session));
                            Response.Redirect("home", false);
                        }
                        else
                        {
                            string cVenName = SafeConvert.SafeString(dt.Rows[0]["cVenName"], "");
                            string expiredTime = DateTime.Now.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss");
                            session = Utils.Utils.SHA256Encryptor(JsonConvert.SerializeObject(new { cVenCode, expiredTime }));
                            if (ZYSoft.DB.BLL.Common.ExecuteNonQuery(string.Format(Const.SQL_INSERT_SESSION, session, cVenCode, cVenName, expiredTime)) > 0)
                            {
                                Thread.Sleep(300);
                                Response.Cookies.Add(new HttpCookie("session", session));
                                Response.Redirect("home", false);
                            }
                        }
                    }
                }
            }
        }
    }
}