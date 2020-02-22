using Atlas.DAL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Atlas.DataAccess.Entity
{
    public static class Common
    {
        private static string _myConnection;

        static Common()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }
        public static List<Setup97_ZipCodes> getStateAndCityZip(string zipCode = "")
        {
            List<Setup97_ZipCodes> lstCityZip = new List<Setup97_ZipCodes>();

            SqlDataReader zipResult = null;

            if (!string.IsNullOrWhiteSpace(zipCode))
            {
                zipResult = SqlHelper.ExecuteReader(_myConnection, "spATL_CRM_Get_ZipCode",
                    new SqlParameter("@Zipcode", zipCode));
            }
            else
            {
                zipResult = SqlHelper.ExecuteReader(_myConnection, CommandType.Text, "Select * from Setup97_ZipCodes");
            }

            while (zipResult.Read())
            {
                Setup97_ZipCodes objZipCodes = new Setup97_ZipCodes();
                objZipCodes.State = Convert.ToString(zipResult["State"]);
                objZipCodes.Town = Convert.ToString(zipResult["Town"]);
                objZipCodes.StateName = Convert.ToString(zipResult["StateName"]);
                objZipCodes.Zipcode = Convert.ToString(zipResult["Zipcode"]);

                lstCityZip.Add(objZipCodes);
            }
            zipResult.Close();
            return lstCityZip;
        }

        public static dynamic getStates()
        {
            List<Setup97_ZipCodes> stateCollection = Common.getStateAndCityZip().AsEnumerable().ToList();
            var lstState = (from item in stateCollection
                            select new
                            {
                                stateid = item.State,
                                statename = item.State
                            })
                              .ToList()
                              .Distinct();

            return lstState;
        }

        public static List<Setup97_ZipCodes> getStatesList()
        {
            List<Setup97_ZipCodes> lstStates = new List<Setup97_ZipCodes>();

            SqlDataReader StatesResult = null;
            StatesResult = SqlHelper.ExecuteReader(_myConnection, "spddl_State");

            while (StatesResult.Read())
            {
                Setup97_ZipCodes objStates = new Setup97_ZipCodes();
                objStates.State = Convert.ToString(StatesResult["State"]);
                objStates.StateName = Convert.ToString(StatesResult["StateName"]);

                lstStates.Add(objStates);
            }
            StatesResult.Close();
            return lstStates;
        }

        public static bool CheckModelIsEmpty(object myObject)
        {
            int i = 1;
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        i++;
                    }
                }
                else if (pi.PropertyType == typeof(Int32))
                {
                    int value = (int)pi.GetValue(myObject);
                    if (value == 0)
                    {
                        i++;
                    }
                }
            }
            return !(myObject.GetType().GetProperties().Count() == i);
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = ConfigurationManager.AppSettings["Encryptkey"].ToString();
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = ConfigurationManager.AppSettings["Encryptkey"].ToString();
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string FormatPhoneText(string value)
        {
            return
            !string.IsNullOrWhiteSpace(value)
                                                       ? new string(value.
                                                         Where(x => char.IsDigit(x)).
                                                         ToArray()) : null;
        }

        public static string ToTitleCase(this string Phrase)
        {
            if (!string.IsNullOrWhiteSpace(Phrase))
            {
                // value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
                MatchCollection Matches = Regex.Matches(Phrase, "\\b\\w");
                Phrase = Phrase.ToLower();
                foreach (Match Match in Matches)
                    Phrase = Phrase.Remove(Match.Index, 1).Insert(Match.Index, Match.Value.ToUpper());

                return Phrase;
            }
            else
            {
                Phrase = string.Empty;
            }
            return Phrase;
        }

        public static List<INV07_UnitOfMeasure> getUnitofMeasure()
        {
            var lstOfUnits = new List<INV07_UnitOfMeasure>();
            var result = SqlHelper.ExecuteDataset(_myConnection,CommandType.Text, "SELECT UnitOfMeasure, UomDescription FROM INV07_UnitOfMeasure").Tables[0].AsEnumerable();
            foreach (var item in result)
            {
                INV07_UnitOfMeasure source = new INV07_UnitOfMeasure();
                source.UnitOfMeasure = Convert.ToString(item["UnitOfMeasure"]);
                source.UomDescription = Convert.ToString(item["UomDescription"]);
                lstOfUnits.Add(source);
              
            }
            return lstOfUnits;
        }
    }
}