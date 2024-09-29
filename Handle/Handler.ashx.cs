using HuakeWeb.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

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
            string errMsg; Dictionary<string, object> dic;
            switch (action)
            {
                case "notice":
                    string type = context.Request.QueryString["type"] ?? "1";//正常通知
                    break;
                case "query-vendor":
                    if (Utils.Utils.ReadStrem2Dic(context.Request.InputStream, out dic, out errMsg))
                    {
                        string code = SafeConvert.SafeString(dic["code"], "");
                        string name = SafeConvert.SafeString(dic["name"], "");
                        if (!ZYSoft.DB.BLL.Common.Exist(string.Format(Const.SQL_VERFIY_VENDOR, code, name)))
                        {
                            context.Response.Write(AjaxResult.fail("不存在供应商或供应商已被绑定"));
                        }
                        else
                        {
                            context.Response.Write(AjaxResult.success(""));
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
                        string startDate = SafeConvert.SafeString(dic["startDate"], "");
                        string endDate = SafeConvert.SafeString(dic["endDate"], "");
                        DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_QUERY_RECORD, "99033", startDate, endDate));
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
                default:
                    context.Response.Write(AjaxResult.fail("没有指定操作类型"));
                    break;
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