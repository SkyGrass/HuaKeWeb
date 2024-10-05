using HuakeWeb.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using static System.Collections.Specialized.BitVector32;

namespace HuakeWeb.Handle
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json;utf-8";
            string action = (context.Request.QueryString["action"] ?? "").ToLower();
            string errMsg = ""; Dictionary<string, object> dic;
            switch (action)
            {
                case "send-msg":
                    int tid = SafeConvert.SafeInt(context.Request.QueryString["type"] ?? "1", 1);//正常通知
                    if (tid == 1)
                        SendMsg1(context, errMsg);
                    else if (tid == 2)
                        SendMsg2(context, errMsg);
                    break;
                case "bind":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        string code = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "code", ""), "");
                        string openId = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "openId", ""), "");
                        if (string.IsNullOrEmpty(openId))
                        {
                            context.Response.Write(AjaxResult.fail("绑定失败,原因：没有获取到微信识别身份"));
                        }
                        else
                        {
                            if (ZYSoft.DB.BLL.Common.ExecuteNonQuery(string.Format(Const.SQL_BIND_USER, code, openId)) > 0)
                            {
                                context.Response.Write(AjaxResult.success("绑定成功"));
                            }
                            else
                            {
                                context.Response.Write(AjaxResult.fail("绑定失败,原因：没有查询到供应商信息或已被绑定"));
                            }
                        }
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.fail(errMsg));
                    }
                    break;
                case "unbind":
                    string userCode = Utils.Utils.GetUserCodeBySession(context);
                    if (!string.IsNullOrEmpty(userCode))
                    {
                        if (ZYSoft.DB.BLL.Common.ExecuteNonQuery(string.Format(Const.SQL_UNBIND_USER, userCode)) > 0)
                        {
                            try
                            {
                                string userSession = Utils.Utils.GetCookie(context.Request, "session", "");
                                ZYSoft.DB.BLL.Common.ExecuteNonQuery(string.Format(Const.SQL_DELETE_USER_SESSION, userSession));
                                context.Request.Cookies.Clear();
                            }
                            catch (Exception)
                            {
                            }
                            context.Response.Write(AjaxResult.success("解绑成功"));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("解绑失败,原因：没有查询到供应商信息或未被绑定"));
                        }
                    }
                    break;
                case "user-info":
                    string session = Utils.Utils.GetCookie(context.Request, "session", "");
                    if (!string.IsNullOrEmpty(session))
                    {
                        DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_USER_INFO, session));
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            context.Response.Write(AjaxResult.success("", dt));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("您尚未登录或者您的登录已失效"));
                        }
                    }
                    else
                        context.Response.Write(AjaxResult.fail("您尚未登录或者您的登录已失效"));
                    break;
                case "query-vendor":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        string code = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "code", ""), "");
                        string name = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "name", ""), "");
                        if (ZYSoft.DB.BLL.Common.Exist(string.Format(Const.SQL_VERFIY_VENDOR, code, name)))
                        {
                            context.Response.Write(AjaxResult.success(""));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("不存在供应商或供应商已被绑定"));
                        }
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.fail(errMsg));
                    }
                    break;
                case "query-record":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        userCode = Utils.Utils.GetUserCodeBySession(context);
                        string id = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "id", ""), "");
                        string startDate = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "startDate", ""), "");
                        string endDate = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "endDate", ""), "");
                        DataTable dt = !string.IsNullOrEmpty(id) ? ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_QUERY_RECORD_BY_ID, id, userCode)) :
                            ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_QUERY_RECORD, userCode, startDate, endDate));
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            context.Response.Write(AjaxResult.success("", dt));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("没有查询到运费结算单"));
                        }
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.fail(errMsg));
                    }
                    break;
                case "query-detail":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        string id = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "id", ""), "");
                        DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_QUERY_RECORD_DETAIL, id));
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            context.Response.Write(AjaxResult.success("", dt));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("没有查询到运费结算单详情"));
                        }
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.fail(errMsg));
                    }
                    break;
                case "save-item":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        string id = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "id", ""), "");
                        string prop = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "prop", ""), "");
                        string keyword = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "keyword", ""), "");
                        if (ZYSoft.DB.BLL.Common.ExecuteNonQuery(string.Format(Const.SQL_UPDATE_RECORD_ITEM, string.Format(@"{0}='{1}'", prop, keyword), id)) > 0)
                        {
                            context.Response.Write(AjaxResult.success("操作成功"));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("操作失败"));
                        }
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.fail(errMsg));
                    }
                    break;
                case "query-img":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        string id = SafeConvert.SafeString(SafeConvert.SafeDictionry(dic, "id", ""), "");
                        DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_QUERY_IMG, id));
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            context.Response.Write(AjaxResult.success("", dt));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("没有查询到图片信息"));
                        }
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.fail(errMsg));
                    }
                    break;
                case "save-img":
                    try
                    {
                        HttpPostedFile file = context.Request.Files["file"];
                        string rowId = SafeConvert.SafeString(context.Request.Form["rowId"], ""); //单据id
                        string mediaId = SafeConvert.SafeString(context.Request.Form["mediaId"], "");
                        string fileType = "." + (SafeConvert.SafeString(context.Request.Form["fileType"], "png").Split('/')).LastOrDefault();
                        string path = context.Server.MapPath("~/upload");
                        path = Path.Combine(path, rowId);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = Path.Combine(path, Utils.Utils.GetRandomString(10, useSpe: false) + fileType);
                        file.SaveAs(path);
                        path = path.Replace(context.Server.MapPath("~"), "");
                        string autoId = ZYSoft.DB.BLL.Common.ExecuteScalar(string.Format(Const.SQL_SAVE_IMG, rowId, path));
                        if (SafeConvert.SafeInt(autoId, 0) > -1)
                        {
                            context.Response.Write(AjaxResult.success("操作成功", new { autoId, path }));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("没有查询到资源图"));
                        }
                    }
                    catch (HttpException ex)
                    {
                        context.Response.Write(AjaxResult.fail(ex.Message));
                    }
                    break;
                case "delete-img":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        string id = SafeConvert.SafeString(dic["id"], "");
                        string filePath = "";
                        DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_SELECT_IMG_BY_ID, id));
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            filePath = SafeConvert.SafeString(dt.Rows[0]["cPictureName"], "");
                        }

                        if (ZYSoft.DB.BLL.Common.ExecuteNonQuery(string.Format(Const.SQL_DELETE_IMG, id)) > -1)
                        {
                            context.Response.Write(AjaxResult.success("操作成功"));
                            try
                            {
                                filePath = Path.Combine(context.Server.MapPath("~"), filePath);
                                if (File.Exists(filePath))
                                    File.Delete(filePath);
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.fail("没有查询到资源图"));
                        }
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.fail(errMsg));
                    }
                    break;
                default:
                    context.Response.Write(AjaxResult.fail("暂不支持此操作"));
                    break;
            }
        }

        private void SendMsg1(HttpContext context, string errMsg)
        {
            int tid = 1;
            string bid = SafeConvert.SafeString(context.Request.QueryString["id"], "");
            DataTable dtRecord = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_SELECT_MSG_CONTENT, bid));
            if (dtRecord != null && dtRecord.Rows.Count > 0)
            {
                string content = string.Format(@"{0}|{1}|{2}|{3}|{4}", dtRecord.Rows[0]["cCode"], dtRecord.Rows[0]["cVenName"],
                    dtRecord.Rows[0]["cAddress"], SafeConvert.SafeDecimal(dtRecord.Rows[0]["iNum"], 0).ToString("F0"), SafeConvert.SafeDecimal(dtRecord.Rows[0]["iYFMoney_End"], 0).ToString("F2"));
                string vendorCode = SafeConvert.SafeString(dtRecord.Rows[0]["cVenCode"], "");
                string userOpenId = ZYSoft.DB.BLL.Common.ExecuteScalar(string.Format(Const.SQL_SELECT_VENDOR_OPENID, vendorCode));
                if (!string.IsNullOrEmpty(userOpenId))
                {
                    if (new AppHelper(context).PushMsg(tid, userOpenId, content, string.Format(@"id={0}", bid), ref errMsg))
                    {
                        context.Response.Write(AjaxResult.success("发送成功"));
                    }
                    else
                    {
                        context.Response.Write(AjaxResult.success("发送失败，原因：" + errMsg));
                    }
                }
                else
                {
                    context.Response.Write(AjaxResult.success("发送失败，原因：没有查询到供应商绑定记录"));
                }
            }
            else
            {
                context.Response.Write(AjaxResult.success("发送失败，原因：没有查询到单据记录"));
            }
        }

        private void SendMsg2(HttpContext context, string errMsg)
        {
            int tid = 2;
            try
            {
                string bid = SafeConvert.SafeString(context.Request.QueryString["id"], "");
                DataTable dtRecord = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_SELECT_MSG_CONTENT, bid));
                if (dtRecord != null && dtRecord.Rows.Count > 0)
                {
                    string content = string.Format(@"{0}|{1}|{2}|{3}|{4}", dtRecord.Rows[0]["cCode"], dtRecord.Rows[0]["cVenName"],
                        dtRecord.Rows[0]["cAddress"], SafeConvert.SafeDecimal(dtRecord.Rows[0]["iNum"], 0).ToString("F0"), SafeConvert.SafeDecimal(dtRecord.Rows[0]["iYFMoney_End"], 0).ToString("F2"));
                    string vendorCode = SafeConvert.SafeString(dtRecord.Rows[0]["cVenCode"], "");
                    string userOpenId = ZYSoft.DB.BLL.Common.ExecuteScalar(string.Format(Const.SQL_SELECT_VENDOR_OPENID, vendorCode));
                    if (!string.IsNullOrEmpty(userOpenId))
                    {
                        if (new AppHelper(context).PushMsg(tid, userOpenId, content, string.Format(@"id={0}", bid), ref errMsg))
                        {
                            List<string> filePaths = new List<string>();
                            List<string> sqls = new List<string>
                            {
                                string.Format(Const.SQL_CLEAR_RECORD, bid)
                            };
                            DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_QUERY_RECORD_DETAIL, bid));
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                string path = context.Server.MapPath("~/upload");
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string ids = SafeConvert.SafeString(dr["AutoID"], "");
                                    sqls.Add(string.Format(Const.SQL_CLEAR_IMG, ids));
                                    if (Directory.Exists(Path.Combine(path, ids)))
                                    {
                                        filePaths.Add(Path.Combine(path, ids));
                                    }
                                }
                            }
                            if (ZYSoft.DB.BLL.Common.ExecuteSQLTran(sqls) > 0)
                            {
                                try
                                {
                                    filePaths.ForEach(path =>
                                    {
                                        Directory.Delete(path, true);
                                    });
                                    context.Response.Write(AjaxResult.success("发送成功"));
                                }
                                catch (Exception)
                                {
                                    context.Response.Write(AjaxResult.success("发送成功,但文件未能删除"));
                                }
                            }
                            else
                            {
                                context.Response.Write(AjaxResult.success("发送失败，原因：清空记录未成功"));
                            }
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.success("发送失败，原因：" + errMsg));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(AjaxResult.success("发送失败，原因：" + ex.Message));
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}