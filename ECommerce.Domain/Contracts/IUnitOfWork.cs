using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.AppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>;

        //IGenericRepository<AppUser, int> Users { get; }
        IUserRepository Users { get; set; }


    }
}
