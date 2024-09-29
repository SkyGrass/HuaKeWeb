using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HuakeWeb
{
    public class Const
    {
        public const string SQL_VERFIY_TOKEN = @"select * from ZYSoftUserSession Where token = '{0}'";
    }
}