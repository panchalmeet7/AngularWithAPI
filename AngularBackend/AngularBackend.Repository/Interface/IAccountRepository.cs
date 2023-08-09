using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace AngularBackend.Repository.Interface
{
    public interface IAccountRepository
    {
        Task<JsonResult> RegisterUser(RegisterViewModel registerViewModel);
        Task<JsonResult> LoginUser(UserViewModel userViewModel);
        Task<JsonResult> SendEmail(string email);
        Task<JsonResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel);
    }
}
