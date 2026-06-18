using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Kenshu_DocumentConfiguration : IEntityTypeConfiguration<T_Kenshu_Document>
{
    public void Configure(EntityTypeBuilder<T_Kenshu_Document> builder)
    {
        builder.ToTable("T_KENSHU_DOCUMENT");
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID).ValueGeneratedOnAdd();
        builder.Property(e => e.DOCUMENTNAME).HasMaxLength(256).IsRequired();
        builder.Property(e => e.DOCUMENTDIR).HasMaxLength(256).IsRequired();

        builder.HasOne(e => e.Kenshu)
            .WithMany(k => k.KenshuDocuments)
            .HasForeignKey(e => e.KENSHUCD);
    }
}