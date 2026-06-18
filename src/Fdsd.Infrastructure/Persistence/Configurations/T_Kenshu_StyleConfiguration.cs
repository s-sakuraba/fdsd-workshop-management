using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Kenshu_StyleConfiguration : IEntityTypeConfiguration<T_Kenshu_Style>
{
    public void Configure(EntityTypeBuilder<T_Kenshu_Style> builder)
    {
        builder.ToTable("T_KENSHU_STYLE");
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID).ValueGeneratedNever();
        builder.Property(e => e.NAME).HasMaxLength(30).IsRequired();
        builder.Property(e => e.RYAKUSHO).HasMaxLength(10).IsRequired();
        builder.Property(e => e.BIKO).HasMaxLength(200);
    }
}