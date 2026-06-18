using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class M_GakkaConfiguration : IEntityTypeConfiguration<M_Gakka>
{
    public void Configure(EntityTypeBuilder<M_Gakka> builder)
    {
        builder.ToTable("M_GAKKA");
        builder.HasKey(e => e.GAKKACD);
        builder.Property(e => e.GAKKACD).ValueGeneratedNever();
        builder.Property(e => e.GAKKANAME).HasMaxLength(30).IsRequired();
        builder.Property(e => e.GAKKARYAKU).HasMaxLength(10);
    }
}