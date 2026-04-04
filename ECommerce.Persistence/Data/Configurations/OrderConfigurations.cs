using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Data.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.UserId)
                .IsRequired();

            builder.Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Status)
                .HasConversion<string>(); // 👈 يخزنه كـ string

            // Address (Owned Type)
            //builder.OwnsOne(o => o.Address, a =>
            //{
            //    a.WithOwner();

            //    a.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
            //    a.Property(a => a.LastName).IsRequired().HasMaxLength(100);
            //    a.Property(a => a.Street).IsRequired();
            //    a.Property(a => a.City).IsRequired();
            //    a.Property(a => a.Country).IsRequired();
            //    a.Property(a => a.Phone).IsRequired();
            //});

            // العلاقة مع DeliveryMethod
            builder.HasOne(o => o.DeliveryMethod)
                .WithMany()
                .HasForeignKey(o => o.DeliveryMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع OrderItems
            builder.HasMany(o => o.Items)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
