using Amazon.Auth.AccessControlPolicy;
using Amazon.Runtime.SharedInterfaces;
using BusinessObjects;
using BusinessService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TrinhHuuTuan.Practice.AuthVsMongoDb.Middlewares.Authentication;

namespace TrinhHuuTuan.Practice.AuthVsMongoDb.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        AuthService service = new AuthService();
        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        [Route("{email}")]
        public IActionResult GetByEmail([FromRoute] string email, string token)
        {
            ApiKeyAuthMiddleware  A= new ApiKeyAuthMiddleware(_config);
            if (!A.ValidateToken(token)) {
                return BadRequest("Fail Token");
            }
            var pi = A.GetClaimsPrincipalFromToken(token);
            if (pi != null)
            {
                return BadRequest("Missing token");
            }

            string keyword = email.Trim().ToLower();
            if(string.IsNullOrEmpty(keyword)) {
                return BadRequest("Not fully email"); 
            }
            var user = service.GetByEmail(email);
            return Ok(new { result = pi.Claims, user = user });

        }
        [AllowAnonymous]
        [HttpPost]
        public  IActionResult Login(string email, string password) {
            var user = Authenticate(email,password);
            if (user != null)
            {
                var token = Generate(user);
                return Ok(new { result = true, token });
            }
            return NotFound("User not found");
        }

        private string Generate(UserDTO user)
        {   // Lấy key để tạo token dựa theo key mình cung cấp 
            var sercurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            // Cung cấp phương thức mã hóa UserDTO thành token; 
            var credentials = new SigningCredentials(sercurityKey,SecurityAlgorithms.HmacSha256);
            // Đưa ra các đặc tính của UserDTO để tạo token; 
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            // tạo token 
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private UserDTO Authenticate(string email, string password)
        {
            var currentUser = service.GetByEmail(email); 
            if(!password.Equals(currentUser.Password)) {
                return null;
            }
            return currentUser;
        }
    }
}
