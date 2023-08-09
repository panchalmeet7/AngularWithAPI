using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AngularBackend.Services.ServiceInterface
{
    public interface IUserService
    {
        Task<JsonResult> Register(RegisterViewModel registerViewModel);
        Task<JsonResult> Login(UserViewModel userViewModel);
        Task<JsonResult> SendEmail(string email);
        Task<JsonResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel);
    }
}
