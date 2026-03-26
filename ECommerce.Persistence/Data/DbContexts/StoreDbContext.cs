using ECommerce.Domain.Entities.AppUser;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Persistence.Data.DataSeed;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistence.Data.DbContexts
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options) { }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تطبيق أي configurations موجودة
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Seed بيانات DeliveryMethods
            modelBuilder.Entity<DeliveryMethod>().HasData(
                new DeliveryMethod
                {
                    Id = 1,
                    ShortName = "Standard",
                    Description = "Standard Delivery",
                    DeliveryTime = "3-5 days",
                    Price = 500
                },
                new DeliveryMethod
                {
                    Id = 2,
                    ShortName = "Express",
                    Description = "Fast Delivery",
                    DeliveryTime = "1-2 days",
                    Price = 1000
                }
            );

            // لو عندك أي Seed إضافي
            StoreDataSeed.SeedData(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        //public DbSet<AppUser> Users { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        #region Old_SHAPE
        //public DbSet<ProductBrand> ProductBrands { get; set; }
        //public DbSet<ProductType> ProductTypes { get; set; }
        #endregion

    }
}
