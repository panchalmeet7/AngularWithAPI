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

namespace AngularBackend.Repository.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly GetConnString _getConnString;
        private readonly DummyAppContext _DbContext;
        public AccountRepository(GetConnString getConnString, DummyAppContext DbContext)
        {
            _getConnString = getConnString;
            _DbContext = DbContext;
        }

        #region Registration
        public async Task<JsonResult> RegisterUser(RegisterViewModel registerViewModel)
        {
            try
            {
                var userExist = await Task.FromResult(_DbContext.Users.Where(x => x.Email == registerViewModel.Email).FirstOrDefault());
                if (userExist != null)
                {
                    return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.UserAlreadyExist, StatusCode = ResponceStatusCode.AlreadyExist, Result = false });
                }
                else
                {
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
            }
            catch
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InternalServerError, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }
        }
        #endregion
    }

}
