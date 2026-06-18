using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class K_CodeConfiguration : IEntityTypeConfiguration<K_Code>
{
    public void Configure(EntityTypeBuilder<K_Code> builder)
    {
        builder.ToTable("K_CODE");
        builder.HasKey(e => new { e.CODENO, e.CODE });
        builder.Property(e => e.NAME).HasMaxLength(50).IsRequired();
        builder.Property(e => e.RYAKUSHO).HasMaxLength(10);
        builder.Property(e => e.BIKO).HasMaxLength(200);
    }
}