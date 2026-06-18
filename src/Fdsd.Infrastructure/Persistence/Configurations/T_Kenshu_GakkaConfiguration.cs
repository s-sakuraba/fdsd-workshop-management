using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Kenshu_GakkaConfiguration : IEntityTypeConfiguration<T_Kenshu_Gakka>
{
    public void Configure(EntityTypeBuilder<T_Kenshu_Gakka> builder)
    {
        builder.ToTable("T_KENSHU_GAKKA");
        builder.HasKey(e => new { e.KENSHUCD, e.GAKKACD });

        builder.HasOne(e => e.Kenshu)
            .WithMany(k => k.KenshuGakkas)
            .HasForeignKey(e => e.KENSHUCD);

        builder.HasOne(e => e.Gakka)
            .WithMany(g => g.KenshuGakkas)
            .HasForeignKey(e => e.GAKKACD);
    }
}