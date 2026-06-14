using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Shop.API.Services;
using Shop.Common.DTOs;
using Shop.Database;
using Shop.Entities;

namespace Shop.API.Controllers
{

    [ApiController]
    [Route("auth")]
    public class AuthorizeController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IJWTTokenService _jwtService;
        public AuthorizeController(AppDbContext appDbContext, IJWTTokenService jwtService)
        {
            _appDbContext = appDbContext;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Shop.Common.DTOs.RegisterRequest request)
        {
     
            var user = await _appDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
            if (user!= null)
            {
                return Conflict(new { message = "пользователь с таким Email уже есть" });
            }
            user = new User
            {
                Id=Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RefreshToken=Guid.NewGuid().ToString(),
                RefreshTokenExpiry=DateTime.Now.AddDays(15).ToUniversalTime(),
                
            };
           var cookieOptions= new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(15)
            };
            Response.Cookies.Append("refreshToken", user.RefreshToken, cookieOptions);
            _appDbContext.Users.Add(user);
             await _appDbContext.SaveChangesAsync();
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role);
            return Ok(token);
            
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            var user = await _appDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
            {
                return Conflict(new { message = "такого пользователя не существует" });
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = user.RefreshTokenExpiry
            };
            Response.Cookies.Append("refreshToken", user.RefreshToken, cookieOptions);
            
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role);
            return Ok(token);

        }
        [Authorize]
        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            return Ok();
        }
    }
}
