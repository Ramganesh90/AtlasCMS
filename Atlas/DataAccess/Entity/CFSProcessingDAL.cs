using Atlas.DAL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace Atlas.DataAccess.Entity
{
    public class CFSProcessingDAL
    {
        private static string _myConnection;
        //const int FILE_DATE_LINE = 3;
        //const int COST_LINE = 4;
        //const int SOLD_FOR_LINE = 7;
        //const int CUSTNAME_LINE = 10;
        //const int PHONENUM_LINE = 11;
        //const int ADDRESS_LINE = 12;
        //const int TOWN_LINE = 13;
        //const int STATE_LINE = 14;
        //const int ZIP_LINE = 15;
        //const int END_OF_HEADER = 16;
        //const int FIRST_REAL_LINE = 20;
        //const int INV_QTY_LINE = 1;
        //const int CALC_FT_PIPE_LINE = 2;
        //const int QTY_LINE = 3;
        //const int CFS_RECORD_LINE = 4;
        //const int ITEM_NUM_LINE = 5;
        //const int DESCR_LINE = 6;
        //const int UOM_LINE = 7;
        //const int SELL_PRICE_LINE = 8;
        //const int HANDLING_LINE = 9;
        //const int STD_WT_LINE = 10;
        //const int IN_YARD_COST_LINE = 11;
        //const int DISCOUNT_FACTOR_LINE = 12;
        //const int EXTRA_LINE13 = 13;
        //const int EXTRA_LINE14 = 14;
        //const int EXTRA_LINE15 = 15;
        //const int EXTRA_LINE16 = 16;
        //const int EXTRA_LINE17 = 17;
        //const int EXTRA_LINE18 = 18;

        static CFSProcessingDAL()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }
        public static int saveCFSFile(HttpPostedFileBase file, int materialID, bool isCostOveridden, string bidid, string cfsDataPath = "")
        {
            int result = 0;
            try
            {
                System.IO.StreamReader iStream = new System.IO.StreamReader(file.InputStream);
                List<string> strLines = new List<string>();
                while (!iStream.EndOfStream)
                {
                    // reading 1 line of datafile
                    strLines.Add(iStream.ReadLine());
                }
                int i = 0, x = 1, n = 0, g = 0, fab_count = 0, line = 0;

                byte fab_exist, IsFab = 0;
                string version = strLines[0];
                BID04_MaterialDetail materialDtl = new BID04_MaterialDetail();

                DataSet ds = new DataSet();
                ds.ReadXml(cfsDataPath);
                DataTable tblLine = ds.Tables[0];
                DataTable tblUOM = ds.Tables[1];


                if (isCostOveridden)
                {
                    string query = "UPDATE [dbo].[BID03_MaterialHeader] SET [OverRiddenCost] ='" + strLines[3] + "'  WHERE [BIDMatHeaderID] =" + materialID;
                    SqlHelper.ExecuteNonQuery(_myConnection, CommandType.Text, query);
                }

                #region CFS File
                for (int counter = 0; counter <= strLines.Count; counter++)
                {

                    //We're skipping lines we don't care about... there's a lot of them.
                    if (counter < GetLineNum(tblLine, "FIRST_REAL_LINE"))
                    {
                    }
                    else
                    {
                        if (x == GetLineNum(tblLine, "INV_QTY_LINE"))
                        {
                            var InvoiceQuantity = strLines[counter - 1];
                        }
                        if (x == GetLineNum(tblLine, "CALC_FT_PIPE_LINE"))
                        {
                            var CalcFtPipeLine = strLines[counter - 1];
                        }
                        if (x == GetLineNum(tblLine, "CFS_RECORD_LINE"))
                        {
                            var s = strLines[counter - 1];
                        }
                        if (x == GetLineNum(tblLine, "QTY_LINE"))
                        {
                            var qty = strLines[counter - 1];
                            if (string.IsNullOrWhiteSpace(qty) || Int32.Parse(qty).Equals(0))
                                break;
                            materialDtl.Qty = Convert.ToDecimal(qty);
                        }

                        if (x == GetLineNum(tblLine, "ITEM_NUM_LINE"))
                        {
                            var itm = Convert.ToString(FormatLine(strLines[counter - 1]));
                            if (itm == "FABRICATED")
                            {
                                materialDtl.ImportFlag = 1;
                                //Throw up a flag for fabricated items.
                                fab_exist = 1;
                                //Don't forget to convert them to gate parts.
                                fab_count = fab_count + 1;
                                IsFab = 1;
                            }
                            if (itm == "NONE")
                            {
                                //Flag for "NONE" in the item number field.
                                materialDtl.ImportFlag = 1;
                            }
                            if ((itm == "CONCPOUR" || itm == "CONCHAND"))
                            {
                                //Flag for "CONCRETE" in the item number field.
                                materialDtl.ImportFlag = 1;
                            }
                            if ((itm == "CLRATE17LBR"))
                            {
                                //Flag for "CLRATE17LBR" in the item number field.
                                materialDtl.ImportFlag = 1;
                            }
                            //oRs("PartNum") = StripChars(itm, "AMS-");
                            materialDtl.PartNum = itm.Replace("AMS-", "");
                        }
                        if (x == GetLineNum(tblLine, "DESCR_LINE"))
                        {
                            string des = Convert.ToString(strLines[counter - 1]);
                            bool bDblGate = false;
                            if (!string.IsNullOrWhiteSpace(des) && des.ToUpper().Substring(0, 6).Equals("SPECIAL"))
                            {
                                //Flag for "SPECIAL" in the begining of description line.
                                materialDtl.ImportFlag = 1;
                            }

                            if (Convert.ToBoolean(IsFab))
                            {
                                string partnum = BuildGatePartNumber(des, out bDblGate);
                                if (partnum.Length != 0)
                                {
                                    materialDtl.PartNum = partnum;
                                    materialDtl.ImportFlag = 0;
                                }
                                IsFab = 0;
                            }

                            if (materialDtl.PartNum.StartsWith("P"))
                            {
                                materialDtl.PartNum = ConvertPNumber(materialDtl.PartNum);
                            }

                            if (bDblGate)
                            {
                                string desc = GetDescription(materialDtl.PartNum) + " **USE AS DOUBLE DRIVE** ";
                                materialDtl.PartNum = Convert.ToString(Int32.Parse(materialDtl.PartNum) * 2);
                                bDblGate = false;
                            }

                            // oRs("ProdLineID") = GetProdLine(oRs("PartNum"));
                            // oRs("ProcurementTypeId") = GetProcureType(oRs("PartNum"));
                            materialDtl.PartNumDesc = des;
                        }
                        //Simple unit of measure notation oConveoRsions.
                        if (x == GetLineNum(tblLine, "UOM_LINE"))
                        {
                            string uom = Convert.ToString(strLines[counter - 1]);
                            materialDtl.UOM = GetUOMDesc(tblUOM, uom);
                        }
                        if (x == GetLineNum(tblLine, "SELL_PRICE_LINE"))
                        {
                            // sellprice = Strings.Trim(iStream.ReadLine);
                            //oRs("sellprice") = sellprice
                        }
                        if (x == GetLineNum(tblLine, "HANDLING_LINE"))
                        {
                            // handling = Strings.Trim(iStream.ReadLine);
                            //oRs("handling") = handling
                            var cost = Convert.ToDecimal(strLines[counter - 1]);
                            materialDtl.Cost = Convert.ToDecimal(cost);
                        }
                        if (x == GetLineNum(tblLine, "STD_WT_LINE"))
                        {
                            // std_wt = Strings.Trim(iStream.ReadLine);
                            //oRs("std_wt") = std_wt
                        }
                        if (x == GetLineNum(tblLine, "IN_YARD_COST_LINE"))
                        {
                            //   yardcost = Strings.Trim(iStream.ReadLine);
                            //oRs("yardcost") = yardcost
                        }
                        if (x == GetLineNum(tblLine, "DISCOUNT_FACTOR_LINE"))
                        {
                            //   discount = Strings.Trim(iStream.ReadLine);
                            //oRs("discount") = discount
                        }

                        if ((version == "8.0W"))
                        {
                            if (x == GetLineNum(tblLine, "EXTRA_LINE14"))
                            {
                            }
                            if (x == GetLineNum(tblLine, "EXTRA_LINE15"))
                            {
                                x = 0;
                            }

                        }
                        else if ((version == "9.0W"))
                        {
                            if (x == GetLineNum(tblLine, "EXTRA_LINE14"))
                            {
                            }

                            if (x == GetLineNum(tblLine, "EXTRA_LINE15"))
                            {
                            }

                            if (x == GetLineNum(tblLine, "EXTRA_LINE16"))
                            {
                            }

                            if (x == GetLineNum(tblLine, "EXTRA_LINE17"))
                            {
                                materialDtl.BIDMatHeaderID = materialID;
                                x = saveSingleCFS(materialDtl);
                                x = 0;

                            }

                        }
                        else if ((version == "10.1W"))
                        {
                            if (x == GetLineNum(tblLine, "EXTRA_LINE14"))
                            {
                            }

                            if (x == GetLineNum(tblLine, "EXTRA_LINE15"))
                            {
                            }

                            if (x == GetLineNum(tblLine, "EXTRA_LINE16"))
                            {
                            }

                            if (x == GetLineNum(tblLine, "EXTRA_LINE17"))
                            {
                                materialDtl.BIDMatHeaderID = materialID;
                                x = saveSingleCFS(materialDtl);
                            }

                            if (x == GetLineNum(tblLine, "EXTRA_LINE18"))
                            {
                                //materialDtl.BIDMatHeaderID = materialID;
                                //x = saveSingleCFS(materialDtl);
                            }

                        }
                        else if (x == GetLineNum(tblLine, "EXTRA_LINE14"))
                        {
                            materialDtl.BIDMatHeaderID = materialID;
                            x = saveSingleCFS(materialDtl);
                            x = 0;
                            materialDtl = new BID04_MaterialDetail();
                            //  iStream.SkipLine        
                        }

                        x = x + 1;
                    }
                }
                #endregion

                result = 1;
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                DeleteExistingCFS(Convert.ToInt32(bidid), 1);
                result = -1;
            }
            return result;
        }
        private static Int32 GetLineNum(DataTable tblLine, string key)
        {

            return Convert.ToInt32(tblLine.AsEnumerable().Where(i => i.Field<string>("RowName").Equals(key)).AsDataView()[0][1]);
        }
        private static string GetUOMDesc(DataTable tblUOM, string key)
        {
            if (!String.IsNullOrWhiteSpace(key))
                return Convert.ToString(tblUOM.AsEnumerable().Where(i => i.Field<string>("shortDesc").Equals(key)).AsDataView()[0][1]);
            else
                return "EA";
        }
        private static int saveSingleCFS(BID04_MaterialDetail materialDtl)
        {
            //Links to the corrisponding header record.
            saveCFSRecord(materialDtl);
            materialDtl = new BID04_MaterialDetail();
            //This update updates a single line item record.
            return 0;
        }
        private static string GetDescription(string desc)
        {
            DataSet readMaster = SqlHelper.ExecuteDataset(_myConnection, CommandType.Text,
             "SELECT PartNum FROM INV01_Master WHERE PartNum = '" + desc + "'");

            var description = readMaster.Tables[0].AsEnumerable().FirstOrDefault();
            return "No Description Found";
        }
        public static void DeleteExistingCFS(int BidID, byte deleteMaterialHdr = 0)
        {
            try
            {
                var resultSet = SqlHelper.ExecuteNonQuery(_myConnection, CommandType.StoredProcedure,
                                      "SP_ATL_DeleteCFSFiles", new SqlParameter("@BidID", BidID),
                                      new SqlParameter("@deleteHeader", deleteMaterialHdr));
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
        }
        internal static int saveCFSRecord(BID04_MaterialDetail material)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();
                spName = "spATL_LBID_MatDtl_Ins";

                parametersList.Add(new SqlParameter("@BIDMatHeaderID", material.BIDMatHeaderID));
                parametersList.Add(new SqlParameter("@PartNum", material.PartNum));
                parametersList.Add(new SqlParameter("@PartNumDesc", material.PartNumDesc));
                parametersList.Add(new SqlParameter("@Qty", material.Qty));
                parametersList.Add(new SqlParameter("@UOM", material.UOM));
                parametersList.Add(new SqlParameter("@Cost", material.Cost));
                parametersList.Add(new SqlParameter("@ImportFlag", material.ImportFlag));


                DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()).Tables[0]);

                result = Convert.ToInt32(resultSet.Rows[0][0]);
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -1;
            }
            return (result);
        }
        private static object FormatLine(string v)
        {
            return !string.IsNullOrWhiteSpace(v) ? Convert.ToString(v).Trim() : string.Empty;
        }
        private static string ConvertPNumber(string sPNumber)
        {
            string sPartNumber = string.Empty;
            var iLength = (dynamic)null;
            // Strip leading 'P'
            sPartNumber = sPNumber.Substring((sPNumber.Length - (sPNumber.Length - 1)));
            if (IsNumber(sPartNumber.Substring(sPartNumber.Length - 3)))
            {
                // Get last 3 characters and convert to feet
                iLength = (Int32.Parse(sPartNumber.Substring((sPartNumber.Length - 3))) / 12);

                bool isNumeric = IsNumber(Convert.ToString(iLength));
                if (isNumeric)
                {
                    iLength = Convert.ToString(iLength);
                    var stepOne = (iLength.Length - (iLength.IndexOf(".", 0, System.StringComparison.OrdinalIgnoreCase) + 1) + 1);
                    if (stepOne > iLength.Length)
                    {
                        if (Convert.ToDouble(iLength.Substring(stepOne - iLength.Length)) <= 0.5)
                        {
                            iLength = Convert.ToString(Int32.Parse(iLength) + "6");
                        }

                    }
                    else
                    {
                        iLength = (Int32.Parse(iLength) + 1);
                    }

                }

                // Strip last few characters
                if (sPartNumber.ToUpper().Contains("SE"))
                {
                    var stepOne = sPartNumber.IndexOf("SE", 0, System.StringComparison.OrdinalIgnoreCase) + 1;
                    if (stepOne > 3)
                    {
                        sPartNumber = sPartNumber.Substring(0, stepOne - 3);
                    }
                }
                else
                {
                    if (sPartNumber.Length > 5)
                    {
                        sPartNumber = sPartNumber.Substring(0, (sPartNumber.Length - 5));
                    }
                }

                DataSet readMaster = SqlHelper.ExecuteDataset(_myConnection, CommandType.Text,
               "SELECT PartNum FROM INV01_Master WHERE PartNum = '" + sPartNumber + (iLength.ToString() + "'"));

                if (readMaster.Tables[0].Rows.Count > 0)
                {
                    sPartNumber = sPartNumber.ToString();
                }
                else
                {
                    sPartNumber = sPNumber;
                }

            }
            else
            {
                sPartNumber = sPNumber;
            }

            return Convert.ToString(sPartNumber);
        }
        public static string BuildGatePartNumber(string sDescription, out bool bDblGate)
        {
            bDblGate = false; try
            {

                sDescription = sDescription.ToLower();
                int NUM_POSITIONS = 7;
                int iPosCounter = 1, iPos1, iPos2;
                string GatePartNumber = string.Empty;
                bool bFoundEquiv = false;
                bool bValidGate = false;
                bool bCantGate = false;

                while (iPosCounter < (NUM_POSITIONS + 1))
                {
                    var reader = SqlHelper.ExecuteReader(_myConnection, CommandType.Text,
                        "SELECT * FROM INV10_GateConvert WHERE Pos = " + iPosCounter);
                    if (reader.HasRows)
                    {
                        while (reader.Read()) //read only registers
                        {
                            string criteria = Convert.ToString(reader["Criteria"]).ToLower();
                            if (sDescription.Contains(criteria))
                            {
                                GatePartNumber = (GatePartNumber + reader["Equiv"]);
                                bFoundEquiv = true;
                            }
                        }
                    }
                    if (!bFoundEquiv)
                    {
                        GatePartNumber = (GatePartNumber + "G");
                    }

                    if ((iPosCounter == 2) && (sDescription.ToUpper().Contains("DOUBLE")))
                    {
                        bDblGate = true;
                    }
                    reader.Close();
                    if (GatePartNumber.ToUpper().EndsWith("CANT"))
                    {
                        iPosCounter = (iPosCounter + 3);
                        bCantGate = true;
                    }
                    else
                    {
                        iPosCounter = (iPosCounter + 1);
                    }

                    if ((iPosCounter == 5))
                    {
                        iPos1 = (sDescription.IndexOf("X", 0, System.StringComparison.OrdinalIgnoreCase) + 1);
                        iPos2 = (sDescription.IndexOf("'", 0, System.StringComparison.OrdinalIgnoreCase) + 1);
                        string str = sDescription.Substring(sDescription.Length - (sDescription.Length - iPos1)).Substring(0, (iPos2 - iPos1) - 1).Trim();
                        Double sRound = 0.0;
                        if (bDblGate)
                        {
                            var str1 = Math.Round((Convert.ToDouble(str) / 2), 0);
                            GatePartNumber += str1;
                            //Double.TryParse(Convert.ToString(s.Length - 1), out sRound);
                            //GatePartNumber = GatePartNumber + Math.Round(sRound / 2);
                        }
                        else
                        {
                            if ((!IsNumber(str) && str.Length > 4))
                            {
                                GatePartNumber = GatePartNumber + Convert.ToString(Math.Round(Convert.ToDouble(str.Length), MidpointRounding.AwayFromZero));
                            }
                            else
                            {
                                GatePartNumber = GatePartNumber + str;
                            }
                        }
                        if (bCantGate)
                        {
                            GatePartNumber = (GatePartNumber + "X");
                            bCantGate = false;
                        }

                        iPosCounter = (iPosCounter + 1);
                    }
                    if ((iPosCounter == 6))
                    {
                        // GatePartNumber = (GatePartNumber + (sDescription.Substring(0, (sDescription.IndexOf(" ") + 1)) - 2) / 12);
                        GatePartNumber += Convert.ToInt32(sDescription.Substring(0, sDescription.Substring(0, (sDescription.IndexOf(" ") + 1)).Length - 2)) / 12;
                        //GatePartNumber = GatePartNumber + (sDescription.Trim().IndexOf(" ", 0) - 2) / 12;
                        iPosCounter = (iPosCounter + 1);
                    }
                }
                var readMaster = SqlHelper.ExecuteReader(_myConnection, CommandType.Text,
                    "SELECT PartNum FROM INV01_Master WHERE PartNum = '" + GatePartNumber + "'");
                while (readMaster.Read())
                {
                    if (readMaster.HasRows)
                        bValidGate = true;
                }
                bDblGate = false;
                if (bValidGate)
                {
                    return GatePartNumber;
                }
                else return "FABRICATED";
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                return "FABRICATED";
            }
        }
        public static List<BID04_MaterialDetail> GetBidMaterialDetails(int id)
        {
            List<BID04_MaterialDetail> materialDtl = new List<BID04_MaterialDetail>();
            var materials = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_LBID_ViewMakeItems",
                new SqlParameter("@matheaderid", id)).Tables[0].AsEnumerable();
            foreach (var item in materials)
            {
                BID04_MaterialDetail material = new BID04_MaterialDetail();
                material.BIDMatDtlID = Convert.ToInt32(item["BIDMatDtlID"]);
                material.BIDMatDtlID = Convert.ToInt32(item["BIDMatDtlID"]);
                material.BIDMatHeaderID = Convert.ToInt32(item["BIDMatDtlID"]);
                material.PartNum = Convert.ToString(item["PartNum"]);
                material.PartNumDesc = Convert.ToString(item["PartNumDesc"]);
                material.Qty = Convert.ToDecimal(item["Qty"]);
                material.UOM = Convert.ToString(item["UOM"]);
                material.Cost = Convert.ToDecimal(item["Cost"]);
                materialDtl.Add(material);
            }

            return materialDtl;

        }
        public static bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern =
            "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern
            + ")|(" + strValidIntegerPattern + ")");
            return !objNotNumberPattern.IsMatch(strNumber) &&
            !objTwoDotPattern.IsMatch(strNumber) &&
            !objTwoMinusPattern.IsMatch(strNumber) &&
            objNumberPattern.IsMatch(strNumber);
        }
    }
}