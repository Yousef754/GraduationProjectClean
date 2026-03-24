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
        // تسجيل مستخدم جديد
        Task<AppUser> RegisterAsync(RegisterUserDto dto);

        // تسجيل الدخول
        Task<AppUser> LoginAsync(LoginUserDto dto);

        // تغيير Role لمستخدم (Admin فقط)
        Task<AppUser> ChangeUserRoleAsync(int userId, string newRole, AppUser currentUser);
    }
}
