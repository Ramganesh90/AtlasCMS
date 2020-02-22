using Atlas.Models;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess.Entity
{
    public class AccountDAL
    {

        private static string _myConnection;

        static AccountDAL()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }
        public static bool RegisterUser(RegisterViewModel UserInfo)
        {
            try
            {
                var result = SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, "spATL_LGN_User_INS",
                    new SqlParameter("@Username", UserInfo.UserName),
                    new SqlParameter("@Password", Common.Encrypt(UserInfo.Password)),
                    new SqlParameter("@FirstName", UserInfo.FirstName.ToTitleCase()),
                    new SqlParameter("@Email", UserInfo.Email),
                    new SqlParameter("@LastName", UserInfo.LastName.ToTitleCase()));
                return (Convert.ToInt32(result) > 0);
            }
            catch(Exception ex)
            {
                Logger.SaveErr(ex);
                return false;
            }
        }

        public static RegisterViewModel Login(LoginViewModel login)
        {
            try
            {
                var result = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_LGN_User_Select",
                    new SqlParameter("@Username", login.UserName),
                    new SqlParameter("@Password", Common.Encrypt(login.Password)));
                RegisterViewModel UserDetails = new RegisterViewModel();
                if(result.Tables[0].Rows.Count > 0)
                {
                    UserDetails.FirstName = Convert.ToString(result.Tables[0].Rows[0]["FirstName"]).ToTitleCase();
                    UserDetails.LastName = Convert.ToString(result.Tables[0].Rows[0]["LastName"]).ToTitleCase();
                    UserDetails.UserName = Convert.ToString(result.Tables[0].Rows[0]["UserName"]);
                  

                }
                if (result.Tables.Count > 1)
                {
                    UserDetails.CommID = result.Tables[1].Rows.Count > 0 ? Convert.ToString(result.Tables[1].Rows[0]["CommID"]) : null;
                    UserDetails.Role = result.Tables[1].Rows.Count > 0 ? Convert.ToString(result.Tables[1].Rows[0]["RoleID"]) : null;
                }

                return UserDetails;
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                return new RegisterViewModel();
            }
        }

        internal static bool CheckforUser(ForgotPasswordViewModel model)
        {
            bool success = false;
            try
            {
                var result = SqlHelper.ExecuteScalar(_myConnection,CommandType.StoredProcedure, "spATL_LGN_ValidateUserForPassword",
                    new SqlParameter("@username",model.UserName),
                    new SqlParameter("@email",model.Email));

                success = Convert.ToBoolean(result);

            }
            catch(Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return success;
        }

        internal static bool SetPasswordForUser(SetPasswordViewModel model, string username,string email)
        {
            bool success = false;
            var validUser = new ForgotPasswordViewModel();
            validUser.UserName = username;
            validUser.Email = email;
            if (AccountDAL.CheckforUser(validUser))
            {
                var result = SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, "spATL_LGN_SetPassword",
                   new SqlParameter("@username", validUser.UserName),
                   new SqlParameter("@email", validUser.Email),
                   new SqlParameter("@password", Common.Encrypt(model.ConfirmPassword)));

                success = Convert.ToBoolean(result);
            }

            return success;
        }


    }
}