using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_KenshuConfiguration : IEntityTypeConfiguration<T_Kenshu>
{
    public void Configure(EntityTypeBuilder<T_Kenshu> builder)
    {
        builder.ToTable("T_KENSHU");
        builder.HasKey(e => e.KENSHUCD);
        builder.Property(e => e.KENSHUCD).ValueGeneratedOnAdd();
        builder.Property(e => e.KENSHUNAME).HasMaxLength(150).IsRequired();
        builder.Property(e => e.SHUSAINAME).HasMaxLength(100).IsRequired();
        builder.Property(e => e.KenshuPlace).HasMaxLength(200);
        builder.Property(e => e.INFODOCU).HasMaxLength(200);

        builder.HasOne(e => e.KenshuStyle)
            .WithMany()
            .HasForeignKey(e => e.KSTYLECD);

        builder.HasIndex(e => e.KENSHUDATE).HasDatabaseName("IX_T_KENSHU_KENSHUDATE");
        builder.HasIndex(e => e.FDSDCD).HasDatabaseName("IX_T_KENSHU_FDSDCD");
    }
}