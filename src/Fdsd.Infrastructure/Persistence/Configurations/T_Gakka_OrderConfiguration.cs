using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Gakka_OrderConfiguration : IEntityTypeConfiguration<T_Gakka_Order>
{
    public void Configure(EntityTypeBuilder<T_Gakka_Order> builder)
    {
        builder.ToTable("T_GAKKA_ORDER");
        builder.HasKey(e => e.GAKKACD);
        builder.Property(e => e.GAKKACD).ValueGeneratedNever();
    }
}