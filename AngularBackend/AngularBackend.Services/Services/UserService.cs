using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Entities.ViewModels;
using AngularBackend.Repository.Interface;
using AngularBackend.Services.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AngularBackend.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IAccountRepository _accountRepository;
        public UserService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<JsonResult> Register(RegisterViewModel registerViewModel)
        {
            return await _accountRepository.RegisterUser( registerViewModel);
        }
        
        public async Task<JsonResult> Login(UserViewModel userViewModel)
        {
            return await _accountRepository.LoginUser(userViewModel);
        }

        public async Task<JsonResult> SendEmail(string email)
        {
            return await _accountRepository.SendEmail(email);
        }

        public async Task<JsonResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            return await _accountRepository.ResetPassword(resetPasswordViewModel);
        }
    }
}
