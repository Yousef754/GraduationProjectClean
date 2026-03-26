using BCrypt.Net;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.AppUser;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Services.Specifications;
using ECommerce.Shared.DTOs.AppUserDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private static readonly string[] AllowedRoles = { "User", "Admin" };

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
            Id = int.Parse(user.Id),
            Email = user.Email,
            Role = "User"
        };
    }

    // ----------------- Login -----------------
    public async Task<AppUser> LoginAsync(LoginUserDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new InvalidOperationException("Invalid Email or Password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            throw new InvalidOperationException("Invalid Email or Password");

        var roles = await _userManager.GetRolesAsync(user);
        return new AppUser
        {
            Id = int.Parse(user.Id),
            Email = user.Email,
            Role = roles.FirstOrDefault() ?? "User"
        };
    }

    // ----------------- Change Role -----------------
    public async Task<AppUser> ChangeUserRoleAsync(int userId, string newRole, AppUser currentUser)
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
            Id = int.Parse(user.Id),
            Email = user.Email,
            Role = newRole
        };
    }
}