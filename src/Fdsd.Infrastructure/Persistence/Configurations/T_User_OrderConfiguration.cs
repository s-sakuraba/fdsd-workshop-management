using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_User_OrderConfiguration : IEntityTypeConfiguration<T_User_Order>
{
    public void Configure(EntityTypeBuilder<T_User_Order> builder)
    {
        builder.ToTable("T_USER_ORDER");
        builder.HasKey(e => e.USERID);
        builder.Property(e => e.USERID).ValueGeneratedNever();
        builder.Property(e => e.EmpName).HasMaxLength(20).IsRequired();
        builder.Property(e => e.GAKKANAME).HasMaxLength(30);
    }
}