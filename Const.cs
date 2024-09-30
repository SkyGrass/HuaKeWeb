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
        public const string SQL_QUERY_RECORD = @"SELECT ID,cCode,dDate,cCardTypeCode,cVouchType,ISNULL(cMemo,'')cMemo,iYFMoney_End ,ISNULL(cCardNo,'')cCardNo,ISNULL(cCardUser,'')cCardUser,ISNULL(cCardUserPhone,'')cCardUserPhone,ISNULL(cSendCode,'')cSendCode,iState FROM 
            [dbo].[Z_CZHK_WLGL_Vouch] T1 WHERE cVenCode ='{0}' AND dDate BETWEEN '{1}' AND '{2} 23:59:59'";
        public const string SQL_QUERY_RECORD_DETAIL = @"SELECT AutoID,ID,ROW_NUMBER() OVER (PARTITION BY ID ORDER BY AutoID) iRowNo,cSource,cSourceCode,cCusName,cAddress,iQuantity, iNum FROM [dbo].[Z_CZHK_WLGL_Vouchs] WHERE ID = '{0}'";
        public const string SQL_UPDATE_RECORD = @"UPDATE [dbo].[Z_CZHK_WLGL_Vouch] SET dCardInDate = '{0}',cCardNo = '{1}',cCardUser = '{2}' ,
            cCardUserPhone = '{3}',cSendCode ='{4}' WHERE iState IN (1,2) AND ID ='{5}";
        public const string SQL_DELETE_IMG = @"DELETE FROM [dbo].[Z_CZHK_WLGL_Vouchs_Picture] WHERE AutoID='{0}'";
        public const string SQL_SAVE_IMG = @"INSERT INTO [dbo].[Z_CZHK_WLGL_Vouchs_Picture] (iIDS,iWxMediaId,cPictureName)VALUES('{0}','{1}','{2}')";
        public const string SQL_QUERY_USER_SESSION = @"select * from ZYSoftUserSession Where session = '{0}'";
    }
}