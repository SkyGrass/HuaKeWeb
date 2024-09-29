using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    }
}