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

        public static SAL01_Company getEditCompany(int id)
        {
            var companies = getAllCompanies().Where(i => i.SalCompId == Convert.ToString(id)
                                                        && i.SalCompActiveFlag.Equals(true));
                
                return companies.FirstOrDefault<SAL01_Company>();
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
                company.SalCompCity = Convert.ToString(record["SalCompCity"]);
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
                var sqlParamsList = new List<SqlParameter>();
                if (!string.IsNullOrWhiteSpace(modelCompany.SalCompId))
                {
                    sqlParamsList.Add(new SqlParameter("@CompId", modelCompany.SalCompId));
                }
                sqlParamsList.Add(new SqlParameter("@Company", modelCompany.SalCompName));
                sqlParamsList.Add(new SqlParameter("@Address", modelCompany.SalCompAddress));
                sqlParamsList.Add(new SqlParameter("@City", modelCompany.SalCompCity));
                sqlParamsList.Add(new SqlParameter("@State", modelCompany.SalCompState));
                sqlParamsList.Add(new SqlParameter("@Zip", modelCompany.SalCompZip));
                sqlParamsList.Add(new SqlParameter("@Phone", modelCompany.SalCompPhone));
                sqlParamsList.Add(new SqlParameter("@Ext", modelCompany.SalCompPhoneExt));
                sqlParamsList.Add(new SqlParameter("@Fax", modelCompany.SalCompFax));
                sqlParamsList.Add(new SqlParameter("@Mobile", modelCompany.SalCompMobile));
                sqlParamsList.Add(new SqlParameter("@EMail", modelCompany.SalCompEMail));
                sqlParamsList.Add(new SqlParameter("@DateEntered", DateTime.Now));
                sqlParamsList.Add(new SqlParameter("@ActiveFlag", modelCompany.SalCompActiveFlag.Value));

                data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_CRM_Comp_Ins", sqlParamsList.ToArray()).Tables[0];

            }
            catch (Exception e)
            {

            }

            return data;
        }


    }
}