using AngularBackend.Common.CommonModels;
using AngularBackend.Entities.Data;
using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Entities.ViewModels;
using AngularBackend.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AngularBackend.Entities.Helper;
using AngularBackend.Common.CommonMethods;
using Microsoft.Extensions.Configuration;

namespace AngularBackend.Repository.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DummyAppContext _DbContext;
        private readonly IConfiguration _config;
        public AccountRepository(DummyAppContext DbContext, IConfiguration config)
        {
            _DbContext = DbContext;
            _config = config;
        }

        #region Login Method
        public async Task<JsonResult> LoginUser(UserViewModel userViewModel)
        {
            var user = await _DbContext.Users
                .FirstOrDefaultAsync(x => x.Email == userViewModel.Email);

            if (user == null)
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserNotFound, StatusCode = ResponceStatusCode.NotFound, Result = false });

            if (!PasswordHasher.VerifyPassword(userViewModel.Password, user.Password))
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InvalidLoginCredentials, StatusCode = ResponceStatusCode.BadRequest, Result = false });

            string token = "";
            token = CommonMethods.CreateToken(user);

            return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.LoginSuccess, StatusCode = ResponceStatusCode.Success, Data = token, Result = true });
        }
        #endregion

        #region Registration Method
        public async Task<JsonResult> RegisterUser(RegisterViewModel registerViewModel)
        {
            try
            {
                var userExist = await Task.FromResult(_DbContext.Users.Where(x => x.Email == registerViewModel.Email).FirstOrDefault());
                if (userExist != null)
                    return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserAlreadyExist, StatusCode = ResponceStatusCode.AlreadyExist, Result = false });

                User user = new()
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    Password = registerViewModel.Password = PasswordHasher.HashPassword(registerViewModel.Password),
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

        #region ForgetPassword Mail Send Method
        public async Task<JsonResult> SendEmail(string email)
        {
            var user = await _DbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

            if (user == null)
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserNotFound, StatusCode = ResponceStatusCode.NotFound, Result = false });

            string token = CommonMethods.CreateTokenForResetPassword();

            string? from = _config["EmailCredentials:From"];
            string body = EmailSubject.EmailStringBody(email, token);
            string subject = "Please Reset your password";
            try
            {
                CommonMethods.SendEmail(from, subject, body);
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

        #region ResetPassword Method
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

            resetPasswordViewModel.ConfirmPassword = user.Password = PasswordHasher.HashPassword(resetPasswordViewModel.NewPassword);
            _DbContext.Entry(user).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.PasswordResetSuccess, StatusCode = ResponceStatusCode.Success, Result = true });
        }
        #endregion
    }

}
