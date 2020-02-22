using Atlas.DAL;
using Atlas.Models;
using Atlas.Models.DBO;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess.Entity
{
    public static class QuoteDal
    {
        private static string _myConnection;

        static QuoteDal()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }

        public static List<Quotes> getQuotesForDisplay(string PRJID)
        {
            var resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                        "spATL_PRJ_Quote_Disp",
                                new SqlParameter("@PRJID", PRJID)).Tables[0].AsEnumerable();

            var QuoteResultList = new List<Quotes>();

            var quotesList = resultSet.Select(i => new PRJ05_Quotes()
            {
                BIDID = i.Field<int>("BIDID"),
                QuoteId = i.Field<int>("QId"),
                QuoteDate = i.Field<DateTime>("QDate"),
                QuoteGroup = i.Field<int>("QGroup"),
                ProjectName = i.Field<string>("ProjectName"),
                BidName = i.Field<string>("BIDName"),
                PRJID = i.Field<int>("PRJID")

            }).ToList<PRJ05_Quotes>();
            var distinctQuotes = quotesList.Select(i => i.QuoteGroup).Distinct();
            foreach (var quoteGroup in distinctQuotes)
            {
                var quotes = new Quotes();
                quotes.QuotesItem = new List<PRJ05_Quotes>();
                var selectQuotes = quotesList.Where(i => i.QuoteGroup == quoteGroup).ToList();
                quotes.QuoteGroup = selectQuotes.FirstOrDefault().QuoteGroup;
                quotes.ProjectName = selectQuotes.FirstOrDefault().ProjectName;
                quotes.PRJID = selectQuotes.FirstOrDefault().PRJID;
                quotes.QuotesItem.AddRange(selectQuotes);
                QuoteResultList.Add(quotes);
            }
            return QuoteResultList;
        }

        internal static int AddQuotesForProject(string pRJID, string bIDID, int QGroup = 0)
        {
            var bidList = bIDID.Split(',');
            if (QGroup == 0)
            {
                string querytoGetQuoteVersion = "Select TOP (1) * from [dbo].[PRJ05_Quotes] Where [PRJID] = @PRJID Order By QGroup Desc";
                var resultData = SqlHelper.ExecuteDataset(_myConnection, CommandType.Text, querytoGetQuoteVersion,
                    new SqlParameter("@PRJID", pRJID)).Tables[0].AsEnumerable();
                var group = resultData.Count() > 0 ? resultData.SingleOrDefault().Field<int>("QGroup") : 0;
                QGroup = group + 1;// Convert.ToInt32(resultData.FirstOrDefault().ToString());
            }
            else
            {
                DeleteQuotes(pRJID, "",QGroup.ToString());
            }

            foreach (var bid in bidList)
            {
                try
                {
                    var result = SqlHelper.ExecuteNonQuery(_myConnection, CommandType.StoredProcedure, "spATL_PRJ_Quote_Ins",
                                   new SqlParameter("@PRJID", pRJID),
                                   new SqlParameter("@BIDID", bid),
                                   new SqlParameter("@QGroup", QGroup));
                }
                catch (Exception ex)
                {
                    Logger.SaveErr(ex);
                    return -2;
                }
            }

            return 0;
        }

        internal static int DeleteQuotes(string pRJID, string bidId = "", string quoteGroup = "")
        {
            int result = 0;
            try
            {
                var parameterList = new List<SqlParameter>();
                parameterList.Add(new SqlParameter("@PRJID", pRJID));
                parameterList.Add(new SqlParameter("@QGROUP", quoteGroup));
                if (!string.IsNullOrWhiteSpace(bidId) && !string.IsNullOrWhiteSpace(quoteGroup))
                {
                    parameterList.Add(new SqlParameter("@BIDID", bidId));
                }

                var resultSet = SqlHelper.ExecuteNonQuery(_myConnection, CommandType.StoredProcedure, "spATL_PRJ_Quote_Del",
                    parameterList.ToArray());

            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -2;
            }
            return result;
        }

        internal static QuoteViewModel GetQuoteByGroup(string prjId, string qGroup)
        {
            var quoteModel = new QuoteViewModel();
            try
            {
                
                var resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                            "spATL_PRJ_Quote_View",
                                    new SqlParameter("@PRJID", prjId),
                                    new SqlParameter("@QGroup", qGroup));


                if (resultSet.Tables.Count > 0) {
                    quoteModel.QuoteList = resultSet.Tables[0].AsEnumerable().Select(i => new PRJ05_Quotes()
                    {
                        BIDID = i.Field<int>("BIDID"),
                        QuoteId = i.Field<int>("QId"),
                        QuoteDate = i.Field<DateTime>("QDate"),
                        QuoteGroup = i.Field<int>("QGroup"),
                        ProjectName = i.Field<string>("ProjectName"),
                        BidName = i.Field<string>("BIDName"),
                        PRJID = i.Field<int>("PRJID")
                    }).ToList<PRJ05_Quotes>();

                    
                    quoteModel.BidHeadersList = resultSet.Tables[1].AsEnumerable().Select(i => new BID01_Headers()
                    {
                        BIDID = i.Field<int>("BIDID"),
                        BIDName = i.Field<string>("BIDName"),
                        BIDStatusID = i.Field<string>("BidStatus"),
                        FenceTypeID = i.Field<int>("FenceTypeID"),
                        FenceHtID = i.Field<int>("FenceHtID"),
                        FtRangeID = i.Field<int>("FtRangeID"),
                        DigTypeID = i.Field<int>("DigTypeID"),
                        TaxCalcTypeID = i.Field<int>("TaxCalcTypeID"),
                        UnitOfMeasure = i.Field<string>("UnitOfMeasure"),
                        DateActivated = i.Field<DateTime>("DateActivated"),
                        DateEntered = i.Field<DateTime>("DateEntered"),
                        QtyOfBI = Convert.ToInt32 (i.Field<decimal>("QtyOfBI"))
                       // SalTxPer = i.Field<int>("SalTxPer"),
                        //CFSFileName = i.Field<int>("FtRangeID"),,
                        //MaterialCost = i.Field<int>("MaterialCost"),

                    }).ToList();

                }
            }
            catch(Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return quoteModel;
        }

    }
}