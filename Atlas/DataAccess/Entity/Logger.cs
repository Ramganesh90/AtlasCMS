using System;
using context = System.Web.HttpContext;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;

namespace Atlas.DataAccess.Entity
{
    public static class Logger
    {
        private static String exepurl;
        private static string _myConnection;

        static Logger()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }
        public static void SaveErr(Exception exdb)
        {
            exepurl = context.Current.Request.Url.ToString();
            var data = SqlHelper.ExecuteNonQuery(_myConnection, CommandType.StoredProcedure, "spATL_ErrorLogs",
                new SqlParameter("@ExceptionMsg", exdb.Message.ToString()),
                new SqlParameter("@ExceptionType", exdb.GetType().Name.ToString()),
                new SqlParameter("@ExceptionURL", exepurl),
                new SqlParameter("@ExceptionSource", exdb.StackTrace.ToString()));
        }
    }
}