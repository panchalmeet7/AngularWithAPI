using AngularBackend.Entities.Data;
using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Entities.ViewModels;
using AngularBackend.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AngularBackend.Common.CommonModels;

namespace AngularBackend.Repository.Repository
{
    public class AccountRepository : IAccountRepository
    {
        #region Properties
        private readonly DummyAppContext _DbContext;
        private readonly IConfiguration _config;
        #endregion

        #region Ctor
        public AccountRepository(DummyAppContext DbContext, IConfiguration config)
        {
            _DbContext = DbContext;
            _config = config;
        }
        #endregion

        #region Login 
        /// <summary>
        /// LoginUser
        /// </summary>
        /// <param name="userViewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> LoginUser(UserViewModel userViewModel)
        {
            var user = await _DbContext.Users
                .FirstOrDefaultAsync(x => x.Email == userViewModel.Email);

            if (user == null)
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserNotFound, StatusCode = ResponceStatusCode.NotFound, Result = false });

            if (!Entities.Helper.PasswordHasher.VerifyPassword(userViewModel.Password, user.Password))
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InvalidLoginCredentials, StatusCode = ResponceStatusCode.BadRequest, Result = false });

            string token = "";
            token = Common.CommonMethods.CommonMethods.CreateToken(user);

            return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.LoginSuccess, StatusCode = ResponceStatusCode.Success, Data = token, Result = true });
        }
        #endregion

        #region Registration 
        /// <summary>
        /// RegisterUser
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> RegisterUser(RegisterViewModel registerViewModel)
        {
            try
            {
                var userExist = await Task.FromResult(_DbContext.Users.Where(x => x.Email == registerViewModel.Email).FirstOrDefault());
                if (userExist != null)
                    return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserAlreadyExist, StatusCode = ResponceStatusCode.AlreadyExist, Result = false });

                User user = new User()
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    Password = registerViewModel.Password = Entities.Helper.PasswordHasher.HashPassword(registerViewModel.Password),
                    Role = "USER",
                    Token = "",
                    PhoneNumber = registerViewModel.PhoneNumber,
                };

                await _DbContext.Users.AddAsync(user);
                await _DbContext.SaveChangesAsync();
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.RegistrationSuccess, StatusCode = ResponceStatusCode.Success, Result = true });
            }
            catch
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InternalServerError, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }
        }
        #endregion

        #region ForgetPassword Mail Send 
        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<JsonResult> SendEmail(string email)
        {
            var user = await _DbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

            if (user == null)
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserNotFound, StatusCode = ResponceStatusCode.NotFound, Result = false });

            string token = Common.CommonMethods.CommonMethods.CreateTokenForResetPassword();

            string? from = _config["EmailCredentials:From"];
            string body = Entities.Helper.EmailSubject.EmailStringBody(email, token);
            string subject = "Please Reset your password";
            try
            {
                Common.CommonMethods.CommonMethods.SendEmail(from, subject, body);
            }
            catch
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.EmailNotSend, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }

            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            _DbContext.Entry(user).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();

            return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.EmailSentSuccessfully, StatusCode = ResponceStatusCode.Success, Result = true });
        }
        #endregion

        #region ResetPassword 
        /// <summary>
        /// ResetPassword
        /// </summary>
        /// <param name="resetPasswordViewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(x => x.Email == resetPasswordViewModel.Email);
            if (user == null)
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserNotFound, StatusCode = ResponceStatusCode.NotFound, Result = false });
            }
            var tokenCode = user.ResetPassewordToken;
            DateTime ExpiryTime = (DateTime)user.ResetPasswordExpiry;

            if (tokenCode != resetPasswordViewModel.EmailToken || DateTime.Now > ExpiryTime)
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.LinkExpired, StatusCode = ResponceStatusCode.RequestFailed, Result = false });
            }

            resetPasswordViewModel.ConfirmPassword = user.Password = Entities.Helper.PasswordHasher.HashPassword(resetPasswordViewModel.NewPassword);
            _DbContext.Entry(user).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.PasswordResetSuccess, StatusCode = ResponceStatusCode.Success, Result = true });
        }
        #endregion
    }

}
