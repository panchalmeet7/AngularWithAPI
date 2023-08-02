using AngularBackend.Entities.Data;
using AngularBackend.Entities.Helper;
using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Repository.Interface;
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
        #endregion

        #region Constructor
        public AccountController(IAccountRepository accountRepository, DummyAppContext DbContext)
        {
            _accountRepository = accountRepository;
            _DbContext = DbContext;
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
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
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
                Message = "Login Successful!!"
            });
        }

  
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            //check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });

            //check pass strength
            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });

            userObj.Password = PasswordHasher.HashPassword(userObj.Password); //password hash
            userObj.Role = "USER";
            userObj.Token = "";
            await _DbContext.Users.AddAsync(userObj);
            await _DbContext.SaveChangesAsync();
            return Ok(new { Message = "Registration Successful!!" });
        }

    }
}
