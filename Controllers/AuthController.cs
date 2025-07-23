
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TowSat_Backend.DTO;
using TowSat_Backend.Models;

namespace TowSat_Backend.Controllers {      

    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {

        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<AppUser> userManager, IConfiguration config) {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (model.Password != model.ConfirmPassword) {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return BadRequest(ModelState);
            }
            var user = new AppUser {
                UserName = model.Username,
                Email = model.Email,
                
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded) {
                return Ok(new { message = "User registered successfully" });
            }
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) {
                ModelState.AddModelError("UserName", "User not found");
                return BadRequest(ModelState);
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password)) {
                return Unauthorized(new { message = "Invalid password" });
            }
            
            
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }



        private string GenerateJwtToken(AppUser user) {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
