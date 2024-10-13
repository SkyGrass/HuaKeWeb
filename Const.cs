using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HuakeWeb
{
    public class Const
    {
        public const string SQL_SELECT_VENDOR_BY_OPENID = @"SELECT cVenCode,cVenName,cVenHand FROM Vendor WHERE isnull(cVenDefine8,'') = '{0}'";
        public const string SQL_VERFIY_VENDOR = @"SELECT cVenCode,cVenName,cVenHand FROM Vendor WHERE cVenCode = '{0}' and cVenName = '{1}'  and cVenHand='{2}' and isnull(cVenDefine8,'') = ''";
        public const string SQL_SELECT_VENDOR_OPENID = @"SELECT isnull(cVenDefine8,'')OpenId FROM Vendor WHERE cVenCode = '{0}'";
        public const string SQL_USER_INFO = @"select UserCode,UserName from ZYSoftUserSession Where UserSession = '{0}' AND ExpiredTime > GETDATE()";
        public const string SQL_USER_SESSION_BY_CODE = @"select UserSession from ZYSoftUserSession Where UserCode = '{0}' AND ExpiredTime > GETDATE()";
        public const string SQL_DELETE_USER_SESSION = @"DELETE FROM  ZYSoftUserSession Where UserSession = '{0}'";
        public const string SQL_INSERT_SESSION = @"insert into ZYSoftUserSession (UserSession,UserCode,UserName,ExpiredTime)Values('{0}','{1}','{2}','{3}')";
        public const string SQL_BIND_USER = @"update Vendor SET cVenDefine8='{1}' WHERE cVenCode ='{0}' AND ISNULL(cVenDefine8,'') =''";
        public const string SQL_UNBIND_USER = @"update Vendor SET cVenDefine8='' WHERE cVenCode ='{0}'";
        public const string SQL_QUERY_RECORD = @"SELECT DISTINCT T1.ID,cCode,dDate,T2.cAddress,ISNULL(dCardInDate,'')dCardInDate,cCardTypeCode,cVouchType,ISNULL(cMemo,'')cMemo,iYFMoney_End ,ISNULL(cCardNo,'')cCardNo,ISNULL(cCardUser,'')cCardUser,ISNULL(cCardUserPhone,'')cCardUserPhone,ISNULL(cSendCode,'')cSendCode,iState FROM [dbo].[Z_CZHK_WLGL_Vouch] T1 LEFT JOIN Z_CZHK_WLGL_Vouchs T2 ON T1.ID = t2.ID WHERE iState IN (1,2) AND cVenCode ='{0}' AND dDate BETWEEN '{1}' AND '{2} 23:59:59'";
        public const string SQL_QUERY_RECORD_BY_ID = @"SELECT DISTINCT T1.ID,cCode,dDate,T2.cAddress,ISNULL(dCardInDate,'')dCardInDate,cCardTypeCode,cVouchType,ISNULL(cMemo,'')cMemo,iYFMoney_End ,ISNULL(cCardNo,'')cCardNo,ISNULL(cCardUser,'')cCardUser,ISNULL(cCardUserPhone,'')cCardUserPhone,ISNULL(cSendCode,'')cSendCode,iState,cVenCode FROM [dbo].[Z_CZHK_WLGL_Vouch] T1 LEFT JOIN Z_CZHK_WLGL_Vouchs T2 ON T1.ID = t2.ID WHERE T1.ID ='{0}' AND cVenCode ='{1}'";
        public const string SQL_QUERY_RECORD_DETAIL = @"
            SELECT T1.AutoID,ID,ROW_NUMBER() OVER (PARTITION BY ID ORDER BY T1.AutoID) iRowNo,cSource,cSourceCode,
            cCusName,cAddress,iQuantity, iNum ,ISNULL(T2.iUploadNum,0)iUploadNum FROM [dbo].[Z_CZHK_WLGL_Vouchs] T1 with (NOLOCK) 
            LEFT JOIN (SELECT COUNT(1)AS iUploadNum,iIDs FROM Z_CZHK_WLGL_Vouchs_Picture with (NOLOCK) GROUP BY iIDs) T2 
            ON T1.AutoID =T2.iIDs WHERE ID = '{0}'";
        public const string SQL_UPDATE_RECORD = @"UPDATE [dbo].[Z_CZHK_WLGL_Vouch] SET dCardInDate = '{1}',cCardNo = '{2}',cCardUser = '{3}' ,
            cCardUserPhone = '{4}',iState=2 WHERE ID ='{0}' AND ISNULL(iState,0) =1";
        public const string SQL_CLEAR_RECORD = @"UPDATE [dbo].[Z_CZHK_WLGL_Vouch] SET dCardInDate = NULL,cCardNo = '',cCardUser = '' ,
            cCardUserPhone = '',cSendCode ='',iState = 1 WHERE ID ='{0}'";
        public const string SQL_CLEAR_IMG = @"DELETE FROM [dbo].[Z_CZHK_WLGL_Vouchs_Picture] WHERE iIDs='{0}'";
        public const string SQL_UPDATE_RECORD_ITEM = @"UPDATE [dbo].[Z_CZHK_WLGL_Vouch] SET {0} WHERE iState IN (2) AND ID ='{1}'";
        public const string SQL_QUERY_IMG = @"SELECT * from Z_CZHK_WLGL_Vouchs_Picture with (NOLOCK)  WHERE iIDs ='{0}'";
        public const string SQL_SELECT_IMG_BY_ID = @"SELECT * FROM [dbo].[Z_CZHK_WLGL_Vouchs_Picture] WHERE AutoID='{0}'";
        public const string SQL_DELETE_IMG = @"DELETE FROM [dbo].[Z_CZHK_WLGL_Vouchs_Picture] WHERE AutoID='{0}'";
        public const string SQL_SAVE_IMG = @"INSERT INTO [dbo].[Z_CZHK_WLGL_Vouchs_Picture] (iIDS,cPictureName)VALUES('{0}','{1}');SELECT @@IDENTITY";
        public const string SQL_SELECT_MSG_CONTENT = @"SELECT T1.cCode,T1.cVenCode,T2.cVenName,T1.iYFMoney_End ,T3.cAddress,SUM(T3.iNum) AS iNum  from [dbo].[Z_CZHK_WLGL_Vouch] T1 with (NOLOCK)
                            LEFT JOIN Vendor T2 with (NOLOCK) ON t1.cVenCode = t2.cVenCode 
                            LEFT JOIN  [dbo].[Z_CZHK_WLGL_Vouchs] T3 with (NOLOCK)
                            ON T1.ID = t3.ID WHERE T1.ID = '{0}' GROUP BY  T1.cCode,T1.cVenCode,T2.cVenName,T1.iYFMoney_End,t3.cAddress";
        public const string SQL_SELECT_CANCEL_MSG_CONTENT = @"SELECT DISTINCT cCode,cVenCode,T2.cAddress,ISNULL(cCardNo,'无')cCardUser,'单据变更,请重新确认'cCancelReason
FROM [dbo].[Z_CZHK_WLGL_Vouch] T1 with (NOLOCK) LEFT JOIN Z_CZHK_WLGL_Vouchs T2 with (NOLOCK) ON T1.ID = t2.ID WHERE T1.ID ='{0}'";
        public const string SQL_SELECT_IMG_IDS = @"SELECT AutoID from Z_CZHK_WLGL_Vouchs with (NOLOCK) WHERE ID='{0}'";
    }
}