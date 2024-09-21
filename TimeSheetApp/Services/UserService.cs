using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeSheetApp.Models;
using TimeSheetApp.Repository;

namespace TimeSheetApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }


        public async Task<User> AddUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            await _userRepository.AddUser(user);
            return user;
        }

        public string Login(Login login)
        {
            var user = _userRepository.GetUser(login);

            if (login.Username != null && login.Password != null)
            {
                if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(login.Password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("Id", user.UserId.ToString()),
                        new Claim("FullName", user.FullName),
                        new Claim("Username", user.Username)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var generateToken = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(1),
                        signingCredentials: signIn
                    );
                    var Token = new JwtSecurityTokenHandler().WriteToken(generateToken);
                    return Token;
                }
            }

            return "Not a Valid User";
        }

        public int GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userIdClaim = user.FindFirst("Id");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("UserId ID not found in claims.");
        }
    }
}
