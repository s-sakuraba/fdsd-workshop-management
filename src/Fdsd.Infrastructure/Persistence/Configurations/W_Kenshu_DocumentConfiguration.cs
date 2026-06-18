using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class W_Kenshu_DocumentConfiguration : IEntityTypeConfiguration<W_Kenshu_Document>
{
    public void Configure(EntityTypeBuilder<W_Kenshu_Document> builder)
    {
        builder.ToTable("W_KENSHU_DOCUMENT");
        builder.HasKey(e => new { e.ID, e.UPDATEUSERID });
        builder.Property(e => e.DOCUMENTNAME).HasMaxLength(256).IsRequired();
        builder.Property(e => e.DOCUMENTDIR).HasMaxLength(256).IsRequired();
    }
}