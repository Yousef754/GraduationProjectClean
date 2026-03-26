using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.AppUser;
using ECommerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistence.Repositories
{
    //public class UserRepository : IUserRepository
    //{
    //    private readonly StoreDbContext _context;

    //    public UserRepository(StoreDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<AppUser?> GetByEmailAsync(string email)
    //    {
    //        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    //    }

    //    public async Task<AppUser?> GetByIdAsync(int id)
    //    {
    //        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    //    }

    //    public async Task AddAsync(AppUser user)
    //    {
    //        await _context.Users.AddAsync(user);
    //    }

    //    public void Update(AppUser user)
    //    {
    //        _context.Users.Update(user);
    //    }
    //}
}
