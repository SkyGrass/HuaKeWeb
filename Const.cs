using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HuakeWeb
{
    public class Const
    {
        public const string SQL_SELECT_VENDOR_BY_OPENID = @"SELECT cVenCode,cVenName,cVenHand FROM Vendor WHERE isnull(cVenDefine8,'') = '{0}'";
        public const string SQL_VERFIY_VENDOR = @"SELECT cVenCode,cVenName,cVenHand FROM Vendor WHERE cVenCode = '{0}' and cVenName = '{1}' and isnull(cVenDefine8,'') = ''";
        public const string SQL_VERFIY_TOKEN = @"select * from ZYSoftUserSession Where token = '{0}'";
        public const string SQL_QUERY_RECORD = @"SELECT ID,cCode,dDate,cCardTypeCode,cVouchType,ISNULL(cMemo,'')cMemo,iYFMoney_End ,ISNULL(cCardNo,'')cCardNo,ISNULL(cCardUser,'')cCardUser,
ISNULL(cCardUserPhone,'')cCardUserPhone,ISNULL(cSendCode,'')cSendCode FROM [dbo].[Z_CZHK_WLGL_Vouch] T1 WHERE cVenCode ='{0}' AND dDate BETWEEN '{1}' AND '{2} 23:59:59'";
        public const string SQL_QUERY_USER_SESSION = @"select * from ZYSoftUserSession Where session = '{0}'";
    }
}