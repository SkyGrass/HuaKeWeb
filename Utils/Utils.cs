using HuakeWeb.Expections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HuakeWeb.Utils
{
    public class Utils
    {
        public static string GetRandomString(int length = 10, bool useNum = true, bool useLow = true, bool useUpp = true,
            bool useSpe = false, string custom = "")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        public static bool ReadStrem2Dic(Stream stream, out Dictionary<string, object> dic, out string errMsg)
        {
            errMsg = "";
            dic = new Dictionary<string, object>();
            using (var reader = new StreamReader(stream))
            {
                try
                {
                    string str = reader.ReadToEnd();
                    dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);
                    return true;
                }
                catch (JsonException)
                {
                    errMsg = "请求入参json错误";
                    return false;
                }
            }
        }

        public static string SHA256Encryptor(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static string GetCookie(HttpRequest request, string key, string defaultValue)
        {
            try
            {
                return SafeConvert.SafeString(request.Cookies[key].Value, "");
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string GetUserCodeBySession(HttpContext content)
        {
            try
            {
                string session = GetCookie(content.Request, "session", "");
                DataTable dt = ZYSoft.DB.BLL.Common.ExecuteDataTable(string.Format(Const.SQL_USER_INFO, session));
                if (null != dt && dt.Rows.Count > 0)
                {
                    return SafeConvert.SafeString(dt.Rows[0]["UserCode"], "");
                }
            }
            catch (NoLoginException e)
            {
                content.Response.Write("<script>alert('您的登录已失效')</script>");
            }
            return "";
        }

        public static async Task DownloadFileAsync(string downloadUrl, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                // 发送HTTP GET请求获取文件内容  
                HttpResponseMessage response = await client.GetAsync(downloadUrl);
                response.EnsureSuccessStatusCode(); // 确保请求成功  

                // 获取响应内容的流  
                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // 将文件内容写入到本地文件  
                    await contentStream.CopyToAsync(fileStream);
                }
            }
        }
    }
}