using Atlas.DAL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess.Entity
{
    public static class CompanyDAL
    {
        private static string _myConnection;

        static CompanyDAL()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }

        public static List<SAL01_Company> getAllCompanies()
        {
            var dataSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.Text, "Select * from SAL01_Company order by SalCompName")
                                   .Tables[0].AsEnumerable().ToList();
            List<SAL01_Company> lstCompanies = new List<SAL01_Company>();
            foreach (var record in dataSet)
            {
                SAL01_Company company = new SAL01_Company();

                company.SalCompId = Convert.ToString(record["SalCompId"]);
                company.SalCompName = Convert.ToString(record["SalCompName"]);
                company.SalCompAddress = Convert.ToString(record["SalCompAddress"]);
                company.SalCompState = Convert.ToString(record["SalCompState"]);
                company.SalCompZip = Convert.ToString(record["SalCompZip"]);
                company.SalCompPhone = Convert.ToString(record["SalCompPhone"]);
                company.SalCompPhoneExt = Convert.ToString(record["SalCompPhoneExt"]);
                company.SalCompFax = Convert.ToString(record["SalCompFax"]);
                company.SalCompMobile = Convert.ToString(record["SalCompMobile"]);
                company.SalCompEMail = Convert.ToString(record["SalCompEMail"]);
                company.SalCompActiveFlag = Convert.ToBoolean(record["SalCompActiveFlag"]);
                lstCompanies.Add(company);
            }

            return lstCompanies;
        }

        public static List<PRJ06_LedSource> getLedSources()
        {
            var dataSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spddl_LedSources")
                                   .Tables[0].AsEnumerable().ToList();
            List<PRJ06_LedSource> lstLedSources = new List<PRJ06_LedSource>();

            foreach (var record in dataSet)
            {
                PRJ06_LedSource source = new PRJ06_LedSource();

                source.LedSourceId = Convert.ToInt32(record["LedSourceId"]);
                source.LedSourceName = Convert.ToString(record["PRJLedSource"]);
                lstLedSources.Add(source);
            }
            return lstLedSources;
        }

        public static DataTable SaveNewCompany(SAL01_Company modelCompany)
        {
            DataTable data = new DataTable();
            try
            {
                data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_CRM_Comp_Ins",
                    new SqlParameter("@Company", modelCompany.SalCompName),
                    new SqlParameter("@Address", modelCompany.SalCompAddress),
                    new SqlParameter("@City", modelCompany.SalCompCity),
                    new SqlParameter("@State", modelCompany.SalCompState),
                    new SqlParameter("@Zip", modelCompany.SalCompZip),
                    new SqlParameter("@Phone", modelCompany.SalCompPhone),
                    new SqlParameter("@Ext", modelCompany.SalCompPhoneExt),
                    new SqlParameter("@Fax", modelCompany.SalCompFax),
                    new SqlParameter("@Mobile", modelCompany.SalCompMobile),
                    new SqlParameter("@EMail", modelCompany.SalCompEMail),
                    new SqlParameter("@DateEntered", DateTime.Now),
                    new SqlParameter("@ActiveFlag", modelCompany.SalCompActiveFlag.Value)).Tables[0];

            }
            catch (Exception e)
            {

            }

            return data;
        }


    }
}