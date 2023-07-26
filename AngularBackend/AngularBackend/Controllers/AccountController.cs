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
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserViewModel userViewModel)
        {
            if (userViewModel == null)
                return BadRequest(); //400 Error

            var user = await _DbContext.Users
                .FirstOrDefaultAsync(x => x.Email == userViewModel.Email && x.Password == userViewModel.Password); //sp to check email and pass
            if (user == null)
                return NotFound(new { Message = "User Not Found!" });

            return Ok(new { Message = "Login Successful!!" });
        }
        #endregion

        #region Register new User
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest();
            //sp to check the email is exist or not
           
            await _DbContext.Users.AddAsync(user);
            await _DbContext.SaveChangesAsync(); //sp to register user
            return Ok(new { Message = "Registration Successful!!" });
        }
        #endregion
    }
}
