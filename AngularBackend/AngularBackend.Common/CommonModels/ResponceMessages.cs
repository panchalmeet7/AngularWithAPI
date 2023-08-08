using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Common.CommonModels
{
    public class ResponceMessages
    {
        #region CommonErrorMessage
        public static string InternalServerError = "Internal Server error";
        public static string DataNotFound = "Data Not Found";
        #endregion

        #region Registration
        public static string RegistrationSuccess = "Registration Successful";
        public static string UserAlreadyExist = "Email already exists";
        #endregion

        #region Login
        public static string LoginSuccess = "Welcome to Dashboard";
        public static string UserNotFound = "User not found";
        public static string InvalidLoginCredentials = "Password is incorrect";
        #endregion

    }
}
