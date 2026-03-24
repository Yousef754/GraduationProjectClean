using ECommerce.Domain.Entities.AppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Contracts
{
    public interface IUserRepository
    {
        // جلب مستخدم بواسطة الايميل (لـ Login أو Register)
        Task<AppUser?> GetByEmailAsync(string email);

        // جلب مستخدم بواسطة الـ Id (لتغيير Role)
        Task<AppUser?> GetByIdAsync(int id);

        // إضافة مستخدم جديد (Register)
        Task AddAsync(AppUser user);

        // تحديث مستخدم موجود (مثلاً لتغيير Role)
        void Update(AppUser user);
    }
}
