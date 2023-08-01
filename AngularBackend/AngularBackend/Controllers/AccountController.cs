using AngularBackend.Entities.Data;
using AngularBackend.Entities.Helper;
using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        
        /// <param name="password"></param>
        /// <returns> Error Msgs </returns>
        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new();
            if(password.Length < 9)
                sb.Append("Minimum password length should be 9" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password Should be Alphanumeric!!" + Environment.NewLine);
            if ((Regex.IsMatch(password, "[<,>,@,!,#,$,&]")))
                sb.Append("Password Should Contain Special Character" + Environment.NewLine);
            return sb.ToString();
        }

        #region Authenticate User by email and password
        /// <summary>
        /// checking email and password
        /// </summary>
        /// <param name="userViewModel"></param>
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserViewModel userViewModel)
        {
            if (userViewModel == null)
                return BadRequest(); //400 Error

            var user = await _DbContext.Users
                .FirstOrDefaultAsync(x => x.Email == userViewModel.Email && x.Password == userViewModel.Password);
            if (user == null)
                return NotFound(new { Message = "User Not Found!" });

            return Ok(new { Message = "Login Successful!!" });
        }
        #endregion

        #region Register new User
        /// <summary>
        /// registering new user into database
        /// </summary>
        /// <param name="user"></param>
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            //check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new {Message = "Email Already Exist!"});

            //check pass strength
            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString()});

            userObj.Password = PasswordHasher.HashPassword(userObj.Password); //password hash
            userObj.Role = "USER";
            userObj.Token = "";
            await _DbContext.Users.AddAsync(userObj);
            await _DbContext.SaveChangesAsync();
            return Ok(new { Message = "Registration Successful!!" });
        }
        #endregion
    }
}
