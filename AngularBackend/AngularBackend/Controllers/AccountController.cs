using AngularBackend.Entities.Data;
using AngularBackend.Entities.Models;
using AngularBackend.Entities.Models.ViewModels;
using AngularBackend.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        #region Authenticate User by email and password
        /// <summary>
        /// checking email and password
        /// </summary>
        /// <param name="userViewModel"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserViewModel userViewModel)
        {
            if (userViewModel == null)
                return BadRequest();

            User userData = new()
            {
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Email = userViewModel.Email,
                PhoneNumber = userViewModel.PhoneNumber,
                Password = userViewModel.Password,
            };
            await _DbContext.Users.AddAsync(userData);
            await _DbContext.SaveChangesAsync();
            return Ok(new { Message = "Registration Successful!!" });
        }
        #endregion
    }
}
