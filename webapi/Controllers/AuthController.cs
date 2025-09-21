using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.Models;
using webapi.ViewModels;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        static List<User> users = new List<User>()
        {
            new User(){ UserName = "prakash@gmail.com", FullName = "prakash", Password = "123", UserId = 1}
            ,new User(){ UserName = "admin@gmail.com", FullName = "Xyz", Password = "1234", UserId = 2}
        };
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(LoginViewModel loginViewModel)
        {
            var user = users.Where(d => d.UserName.ToLower() == loginViewModel.UserName.ToLower() &&
                        d.Password == loginViewModel.Password).FirstOrDefault();
            if (user != null)
            {
                UserViewModel uservm = new UserViewModel()
                {
                    FullName = user.FullName,
                    Token = GenerateJSONWebToken(user),
                    UserName = user.UserName
                };

                return StatusCode(StatusCodes.Status200OK, uservm);
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }


        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                             new Claim(JwtRegisteredClaimNames.Sub, userInfo.FullName),
                             new Claim(JwtRegisteredClaimNames.Email, userInfo.UserName),
                             new Claim(JwtRegisteredClaimNames.Jti, userInfo.UserId.ToString())
                             };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                                            _configuration["Jwt:Audience"],
                                            claims,
                                            expires: DateTime.UtcNow.AddMinutes(60), //token expiry minutes
                                            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
