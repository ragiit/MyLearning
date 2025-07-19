using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Models;
using Order.Domain.ValueObjects;

namespace Order.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order.Domain.Models.Order>
    {
        public void Configure(EntityTypeBuilder<Order.Domain.Models.Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).HasConversion(
                id => id.Value,
                dbId => OrderId.Of(dbId));

            builder.Property(o => o.CustomerId)
                .HasConversion(
                    id => id.Value,
                    dbId => CustomerId.Of(dbId));

            builder.ComplexProperty(o => o.OrderName, nameBuilder =>
            {
                nameBuilder.Property(n => n.Value)
                    .HasColumnName("OrderName")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.ComplexProperty(o => o.Payment, paymentBuilder =>
            {
                paymentBuilder.Property(p => p.PaymentMethod)
                    .HasConversion<string>()
                    .HasColumnName("PaymentMethod")
                    .IsRequired();

                paymentBuilder.Property(p => p.CashAmount)
                    .HasColumnName("CashAmount")
                    .HasPrecision(18, 2);
            });

            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => Enum.Parse<OrderStatus>(s))
                .HasMaxLength(24)
                .IsRequired();

            builder.Property(o => o.Date)
                .IsRequired();

            builder.Ignore(o => o.TotalPrice);

            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(i => i.OrderId);
        }
    }
}