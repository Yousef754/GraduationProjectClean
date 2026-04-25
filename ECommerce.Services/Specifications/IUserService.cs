using ECommerce.Domain.Entities.AppUser;
using ECommerce.Shared.DTOs.AppUserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Specifications
{
    public interface IUserService
    {
        Task<AppUser> RegisterAsync(RegisterUserDto dto);
        Task<(AppUser user, string token)> LoginAsync(LoginUserDto dto);
        Task<AppUser> ChangeUserRoleAsync(string userId, string newRole, AppUser currentUser);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task<AppUser> CreateAdminAsync(CreateAdminDto dto);
        Task LogoutAsync();
        Task<AppUser> GetProfileAsync(string userId);
        Task<AppUser> UpdateDisplayNameAsync(string userId, string newDisplayName);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
    }
}
