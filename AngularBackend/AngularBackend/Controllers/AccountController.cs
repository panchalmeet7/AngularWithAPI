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
        private readonly IEmailRepository _emailRepository;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        #endregion

        #region Constructor
        public AccountController(IAccountRepository accountRepository, DummyAppContext DbContext, IEmailRepository emailRepository, IConfiguration configuration, IUserService userService)
        {
            _accountRepository = accountRepository;
            _DbContext = DbContext;
            _emailRepository = emailRepository;
            _config = configuration;
            _userService = userService;
        }
        #endregion

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

        // token contains 3 things => header, payload and signature
        private string CreateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecretkey.......");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet("Users")]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _DbContext.Users.ToListAsync());
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserViewModel userViewModel)
        {
            if (userViewModel == null)
                return BadRequest();

            var user = await _DbContext.Users
                .FirstOrDefaultAsync(x => x.Email == userViewModel.Email);

            if (user == null)
                return NotFound(new { Message = "User Not Found!" });

            if (!PasswordHasher.VerifyPassword(userViewModel.Password, user.Password))
                return BadRequest(new { Message = "Password is incorrect!" });

            user.Token = CreateToken(user);

            return Ok(new
            {
                Token = user.Token,
                Message = "WELCOME TO THE DASHBOARD!!"
            });
        }


        [HttpPost("Register")]
        public async Task<JsonResult> RegisterUser([FromBody] RegisterViewModel registerViewModel )
        {
            if (ModelState.IsValid)
            {
                var model = await _userService.Register(registerViewModel);
                return model;
            }
            else
            {
                return new JsonResult(new ApiResponce<string> { Message = ResponceMessages.InternalServerError, StatusCode = ResponceStatusCode.BadRequest, Result = false });
            }

            #region Old All in controller method

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

        [HttpPost("send-rest-email/{email}")]
        public async Task<IActionResult> SendEmailToUser(string email)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
            if (user == null)
            {
                return NotFound(new { Message = "Email Doesn't Exist! Please Register First!", StatusCode = 404 });
            }

            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            user.ResetPassewordToken = token;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            string from = _config["EmailCredentials:From"];
            string body = EmailSubject.EmailStringBody(email, token);
            string subject = "Please Reset your password";
            _emailRepository.SendEmail(email, subject, body);
            _DbContext.Entry(user).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return Ok(new { StatusCode = 200, Message = "Email Sent!" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _DbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == resetPasswordViewModel.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User Doesn't Exist! Please Register First!", StatusCode = 404 });
            }

            var tokenCode = user.ResetPassewordToken;
            DateTime ExpiryTime = (DateTime)user.ResetPasswordExpiry;

            if (tokenCode != resetPasswordViewModel.EmailToken || ExpiryTime < DateTime.Now)
            {
                return BadRequest(new { StatusCode = 400, Message = "Invalid Link!" });
            }

            resetPasswordViewModel.ConfirmPassword = user.Password = PasswordHasher.HashPassword(resetPasswordViewModel.NewPassword);
            _DbContext.Entry(user).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();
            return Ok(new { StatusCode = 200, Message = "Password reset successfully!" });
        }

    }
}
