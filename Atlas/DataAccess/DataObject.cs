using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess
{
    public class DataObject : _DataObject
    {
        public string getConnection()
        {
            string ConnStr = ConfigurationManager.ConnectionStrings["AtlasEntities"].ConnectionString;
            return ConnStr;
        }
    }
}