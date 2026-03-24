using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ECommerce.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        // Add
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Delete
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        // Update
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        // Get All
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
        {
            var query = SpecificationEvaluator.CreateQuery(_dbSet, specifications);
            return await query.ToListAsync();
        }

        // Get By Id
        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
        {
            var query = SpecificationEvaluator.CreateQuery(_dbSet, specifications);
            return await query.FirstOrDefaultAsync();
        }

        // Count (for Pagination / Analytics)
        public async Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications)
        {
            var query = SpecificationEvaluator.CreateQuery(_dbSet, specifications);
            return await query.CountAsync();
        }

        // Save changes

        public IQueryable<TEntity> GetAllAsQueryable()
        {
            return _dbContext.Set<TEntity>().AsQueryable();
        }


    }
}
