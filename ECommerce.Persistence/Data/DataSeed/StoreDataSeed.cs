using ECommerce.Domain.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistence.Data.DataSeed
{
    public class StoreDataSeed
    {

        public static void SeedData(ModelBuilder modelBuilder)
        {
            // 1️⃣ Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Mobiles" },
                new Category { Id = 2, Name = "Laptops" },
                new Category { Id = 3, Name = "Accessories" }
            );

            // 2️⃣ Products
            modelBuilder.Entity<Product>().HasData(
                // Mobiles
                new Product { Id = 1, Name = "iPhone 14", Description = "Latest Apple phone", Color = "Black", PictureUrl = "url1", Price = 25000, CategoryId = 1 },
                new Product { Id = 2, Name = "Samsung Galaxy S23", Description = "Flagship Samsung phone", Color = "White", PictureUrl = "url2", Price = 22000, CategoryId = 1 },
                new Product { Id = 3, Name = "Google Pixel 8", Description = "Google's new phone", Color = "Green", PictureUrl = "url3", Price = 20000, CategoryId = 1 },

                // Laptops
                new Product { Id = 4, Name = "Dell XPS 15", Description = "High-end laptop", Color = "Silver", PictureUrl = "url4", Price = 45000, CategoryId = 2 },
                new Product { Id = 5, Name = "MacBook Air M2", Description = "Apple's lightweight laptop", Color = "Gray", PictureUrl = "url5", Price = 55000, CategoryId = 2 },
                new Product { Id = 6, Name = "HP Spectre x360", Description = "Convertible laptop", Color = "Black", PictureUrl = "url6", Price = 40000, CategoryId = 2 },

                // Accessories
                new Product { Id = 7, Name = "AirPods Pro", Description = "Apple wireless earbuds", Color = "White", PictureUrl = "url7", Price = 7000, CategoryId = 3 },
                new Product { Id = 8, Name = "Logitech MX Master 3", Description = "Wireless mouse", Color = "Black", PictureUrl = "url8", Price = 2000, CategoryId = 3 },
                new Product { Id = 9, Name = "Samsung Charger", Description = "Fast charging adapter", Color = "White", PictureUrl = "url9", Price = 500, CategoryId = 3 }
            );
        }
    }
}
