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
    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(i => i.Price)
                .HasColumnType("decimal(18,2)");

            builder.OwnsOne(i => i.Product, p =>
            {
                p.Property(p => p.ProductName)
                    .IsRequired();

                p.Property(p => p.PictureUrl)
                    .IsRequired();
            });
        }
    }
}
