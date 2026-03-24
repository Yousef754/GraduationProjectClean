using BCrypt.Net;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.AppUser;
using ECommerce.Services.Specifications;
using ECommerce.Shared.DTOs.AppUserDTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    private static readonly string[] AllowedRoles = { "User", "Admin" };
    private const int BcryptWorkFactor = 12;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AppUser> RegisterAsync(RegisterUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email and Password are required");

        var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email is already registered");

        var user = new AppUser
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: BcryptWorkFactor),
            Role = "User"
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<AppUser> LoginAsync(LoginUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid Email or Password");

        return user;
    }

    public async Task<AppUser> ChangeUserRoleAsync(int userId, string newRole, AppUser currentUser)
    {
        if (currentUser.Role != "Admin")
            throw new UnauthorizedAccessException("Only Admin can change user roles");

        if (!AllowedRoles.Contains(newRole))
            throw new ArgumentException("Invalid role value");

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        user.Role = newRole;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }
}