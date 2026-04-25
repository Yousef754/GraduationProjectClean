using BCrypt.Net;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.AppUser;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Services;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications;
using ECommerce.Shared.DTOs.AppUserDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    private static readonly string[] AllowedRoles = { "User", "Admin" };

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailService = emailService;

    }

    // ----------------- Register -----------------
    public async Task<AppUser> RegisterAsync(RegisterUserDto dto)
    {
        
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email and Password are required");

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email is already registered");

        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            DisplayName = dto.FullName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        // Assign default role
        await _userManager.AddToRoleAsync(user, "User");

        // تحويل ApplicationUser لـ AppUser للـ Interface
        return new AppUser
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.DisplayName, // ← زود السطر ده

            Role = "User"

        };
    }

    // ----------------- Login -----------------
    public async Task<(AppUser user, string token)> LoginAsync(LoginUserDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new InvalidOperationException("Invalid Email or Password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            throw new InvalidOperationException("Invalid Email or Password");

        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault() ?? "User";

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWTOptions:SecretKey"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, userRole)
        }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["JWTOptions:Issuer"],
            Audience = _configuration["JWTOptions:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        var appUser = new AppUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.DisplayName,
            Role = userRole
        };

        return (appUser, jwtToken);
    }

    // ----------------- Change Role -----------------
    public async Task<AppUser> ChangeUserRoleAsync(string userId, string newRole, AppUser currentUser)
    {
        if (currentUser.Role != "Admin")
            throw new UnauthorizedAccessException("Only Admin can change user roles");

        if (!AllowedRoles.Contains(newRole))
            throw new ArgumentException("Invalid role value");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new InvalidOperationException("User not found");

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, newRole);

        return new AppUser
        {
            Id = user.Id,
            Email = user.Email,
            Role = newRole
        };
    }

    // ----------------- Forgot Password - بيبعت الإيميل -----------------
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new InvalidOperationException("Email not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetEmailAsync(email, token);
    }

    // ----------------- Reset Password - بيغير الباسورد بالـ Token -----------------
    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmNewPassword)
            throw new ArgumentException("Passwords do not match");

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new InvalidOperationException("Email not found");

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // ----------------- Logout -----------------
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    // ----------------- Create Admin -----------------
    public async Task<AppUser> CreateAdminAsync(CreateAdminDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email and Password are required");

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email is already registered");

        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            DisplayName = dto.FullName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole("Admin"));

        await _userManager.AddToRoleAsync(user, "Admin");

        return new AppUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = dto.FullName,
            Role = "Admin"
        };
    }

    // ----------------- Get Profile -----------------
    public async Task<AppUser> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new InvalidOperationException("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        return new AppUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.DisplayName,
            Role = roles.FirstOrDefault() ?? "User"
        };
    }

    // ----------------- Update Display Name -----------------
    public async Task<AppUser> UpdateDisplayNameAsync(string userId, string newDisplayName)
    {
        if (string.IsNullOrWhiteSpace(newDisplayName))
            throw new ArgumentException("Display name cannot be empty");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new InvalidOperationException("User not found");

        user.DisplayName = newDisplayName;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        var roles = await _userManager.GetRolesAsync(user);

        return new AppUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.DisplayName,
            Role = roles.FirstOrDefault() ?? "User"
        };
    }

    // ----------------- Change Password -----------------
    public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmNewPassword)
            throw new ArgumentException("New passwords do not match");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new InvalidOperationException("User not found");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}