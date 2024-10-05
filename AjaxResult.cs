using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuakeWeb
{
    public class AjaxResult
    {
        public AjaxResult(string state, string msg)
        {
            this.state = state;
            this.msg = msg;
        }
        public AjaxResult(string state, string msg, object data)
        {
            this.state = state;
            this.msg = msg;
            this.data = data;
        }

        public string state { get; set; }
        public string msg { get; set; }
        public object data { get; set; }


        public static string success(string msg) =>
            JsonConvert.SerializeObject(new AjaxResult("success", msg));
        public static string fail(string msg) =>
            JsonConvert.SerializeObject(new AjaxResult("error", msg));
        public static string expired() =>
           JsonConvert.SerializeObject(new AjaxResult("expired", "您尚未登录或登录已过期"));

        public static string success(string msg, object data) =>
            JsonConvert.SerializeObject(new AjaxResult("success", msg, data));

    }
}
