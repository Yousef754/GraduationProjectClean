using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.AppUser;
using ECommerce.Services.Specifications;
using ECommerce.Shared.DTOs.AppUserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUserService userService, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        // ----------------- Register -----------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            return Ok(new { user.Id, user.Email, user.Role });
        }

        // ----------------- Login -----------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            var user = await _userService.LoginAsync(dto);
            if (user == null) return Unauthorized("Invalid Email or Password");

            // إنشاء JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWTOptions:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JWTOptions:Issuer"],
                Audience = _configuration["JWTOptions:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Role,
                Token = jwtToken
            });
        }

        // ----------------- Change Role -----------------
        [HttpPut("{userId}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(int userId, [FromBody] string newRole)
        {
            // جلب الـ currentUser من الـ JWT
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var currentUser = await _unitOfWork.Users.GetByEmailAsync(email);
            if (currentUser == null) return Unauthorized();

            var user = await _userService.ChangeUserRoleAsync(userId, newRole, currentUser);
            return Ok(new { user.Id, user.Email, user.Role });
        }
    }
}
