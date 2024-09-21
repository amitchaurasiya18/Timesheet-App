using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeSheetApp.Models;
using TimeSheetApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeSheetApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUser")]
        [Authorize]
        public IActionResult GetUser()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userName = claimsIdentity.FindFirst("Username")?.Value;
                var userId = int.Parse(claimsIdentity.FindFirst("Id")?.Value);
                var fullname = claimsIdentity.FindFirst("FullName")?.Value;

                return Ok(new { Username = userName, UserId = userId, FullName = fullname });
            }
            return Unauthorized();
        }

        [HttpPost("Login")]
        public string Login([FromBody] Login login)
        {
            return _userService.Login(login);
        }

        [HttpPost("AddUser")]
        public Task<User> AddUser([FromBody] User user)
        {
            return _userService.AddUser(user);
        }
    }
}
