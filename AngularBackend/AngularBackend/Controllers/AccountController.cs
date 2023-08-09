using AngularBackend.Common.CommonModels;
using AngularBackend.Entities.Data;
using AngularBackend.Entities.Helper;
using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Entities.ViewModels;
using AngularBackend.Repository.Interface;
using AngularBackend.Services.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace AngularBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region properties
        private readonly IAccountRepository _accountRepository;
        private readonly DummyAppContext _DbContext;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        #endregion

        #region Constructor
        public AccountController(IAccountRepository accountRepository, DummyAppContext DbContext, IConfiguration configuration, IUserService userService)
        {
            _accountRepository = accountRepository;
            _DbContext = DbContext;
            _config = configuration;
            _userService = userService;
        }
        #endregion

        #region Common Methods
        private Task<bool> CheckEmailExistAsync(string email)
             => _DbContext.Users.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new();
            if (password.Length < 9)
                sb.Append("Minimum password length should be 9" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password Should be Alphanumeric!!" + Environment.NewLine);
            //if ((Regex.IsMatch(password, "[<,>,@,!,#,$,&]")))
            //    sb.Append("Password Should Contain Special Character" + Environment.NewLine);
            return sb.ToString();
        }
        #endregion

        #region GetAllUsers API
        [Authorize]
        [HttpGet("Users")]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _DbContext.Users.ToListAsync());
        }
        #endregion

        #region Login API
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                return await _userService.Login(userViewModel);
            }
            else
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InternalServerError, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }

            #region Old Method
            //if (userViewModel == null)
            //    return BadRequest();

            //var user = await _DbContext.Users
            //    .FirstOrDefaultAsync(x => x.Email == userViewModel.Email);

            //if (user == null)
            //    return NotFound(new { Message = "User Not Found!" });

            //if (!PasswordHasher.VerifyPassword(userViewModel.Password, user.Password))
            //    return BadRequest(new { Message = "Password is incorrect!" });

            //user.Token = CreateToken(user);

            //return Ok(new
            //{
            //    Token = user.Token,
            //    Message = "WELCOME TO THE DASHBOARD!!"
            //});
            #endregion
        }
        #endregion

        #region Registration API
        [HttpPost("Register")]
        public async Task<JsonResult> RegisterUser([FromBody] RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                return await _userService.Register(registerViewModel);
            }
            else
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InternalServerError, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }

            #region Old Method

            //if (userObj == null)
            //    return BadRequest();

            ////check email
            //if (await CheckEmailExistAsync(userObj.Email))
            //    return BadRequest(new { Message = "Email Already Exist!" });

            ////check pass strength
            //var pass = CheckPasswordStrength(userObj.Password);
            //if (!string.IsNullOrEmpty(pass))
            //    return BadRequest(new { Message = pass.ToString() });

            //userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            //userObj.Role = "USER";
            //userObj.Token = "";
            //await _DbContext.Users.AddAsync(userObj);
            //await _DbContext.SaveChangesAsync();
            //return Ok(new { Message = "Registration Successful!!" });

            #endregion
        }
        #endregion

        #region ForgetPassword API
        [HttpPost("send-rest-email/{email}")]
        public async Task<IActionResult> SendEmailToUser(string email)
        {
            if (email != null)
            {
                return await _userService.SendEmail(email);
            }
            else
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InternalServerError, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }

            #region Old Method
            //var user = await _DbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
            //if (user == null)
            //{
            //    return NotFound(new { Message = "Email Doesn't Exist! Please Register First!", StatusCode = 404 });
            //}

            //byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            //byte[] key = Guid.NewGuid().ToByteArray();
            //string token = Convert.ToBase64String(time.Concat(key).ToArray());
            //user.ResetPassewordToken = token;
            //user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            //string from = _config["EmailCredentials:From"];
            //string body = EmailSubject.EmailStringBody(email, token); //Via Helper
            //string subject = "Please Reset your password";
            //_emailRepository.SendEmail(email, subject, body);
            //_DbContext.Entry(user).State = EntityState.Modified;
            //await _DbContext.SaveChangesAsync();
            //return Ok(new { StatusCode = 200, Message = "Email Sent!" });
            #endregion
        }
        #endregion

        #region ResetPassword API
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                return await _userService.ResetPassword(resetPasswordViewModel);
            }
            else
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InternalServerError, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }

            #region Old Method
            //var user = await _DbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == resetPasswordViewModel.Email);
            //if (user == null)
            //{
            //    return NotFound(new { Message = "User Doesn't Exist! Please Register First!", StatusCode = 404 });
            //}

            //var tokenCode = user.ResetPassewordToken;
            //DateTime ExpiryTime = (DateTime)user.ResetPasswordExpiry;

            //if (tokenCode != resetPasswordViewModel.EmailToken || ExpiryTime < DateTime.Now)
            //{
            //    return BadRequest(new { StatusCode = 400, Message = "Invalid Link!" });
            //}

            //resetPasswordViewModel.ConfirmPassword = user.Password = PasswordHasher.HashPassword(resetPasswordViewModel.NewPassword);
            //_DbContext.Entry(user).State = EntityState.Modified;
            //await _DbContext.SaveChangesAsync();
            //return Ok(new { StatusCode = 200, Message = "Password reset successfully!" });
            #endregion 
        }
        #endregion

    }
}
