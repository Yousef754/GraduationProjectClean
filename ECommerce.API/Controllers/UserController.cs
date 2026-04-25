using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.AppUser;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Services.Specifications;
using ECommerce.Shared.DTOs.AppUserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

namespace ECommerce.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ----------------- Register -----------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var user = await _userService.RegisterAsync(dto);
                return Ok(new { user.Id, user.Email, user.FullName, Role = "User" });
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------- Login -----------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            try
            {
                var (user, token) = await _userService.LoginAsync(dto);
                return Ok(new { user.Id, user.Email, user.FullName, user.Role, Token = token });
            }
            catch (InvalidOperationException ex) { return Unauthorized(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------- Logout -----------------
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return Ok("Logged out successfully");
        }

        // ----------------- Forgot Password -----------------
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            try
            {
                await _userService.ForgotPasswordAsync(dto.Email);
                return Ok("Reset token sent to your email");
            }
            catch (InvalidOperationException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------- Reset Password -----------------
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _userService.ResetPasswordAsync(dto);
                return Ok("Password reset successfully");
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------- Create Admin -----------------
        [HttpPost("create-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            try
            {
                var admin = await _userService.CreateAdminAsync(dto);
                return Ok(new { admin.Id, admin.Email, admin.FullName, Role = "Admin" });
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------- Get Profile -----------------
        [HttpGet("profile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var profile = await _userService.GetProfileAsync(userId);
                return Ok(new { profile.FullName, profile.Email });
            }
            catch (InvalidOperationException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------- Update Display Name -----------------
        [HttpPut("profile/display-name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDisplayName([FromBody] UpdateDisplayNameDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var updated = await _userService.UpdateDisplayNameAsync(userId, dto.NewDisplayName);
                return Ok(new { updated.FullName, updated.Email, Message = "Name updated successfully" });
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // ----------------- Change Password -----------------
        [HttpPut("profile/change-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _userService.ChangePasswordAsync(userId, dto);
                return Ok("Password changed successfully");
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
