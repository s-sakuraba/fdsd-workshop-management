using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Kenshu_AttendConfiguration : IEntityTypeConfiguration<T_Kenshu_Attend>
{
    public void Configure(EntityTypeBuilder<T_Kenshu_Attend> builder)
    {
        builder.ToTable("T_KENSHU_ATTEND");
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID).ValueGeneratedOnAdd();
        builder.Property(e => e.ATTEND).HasDefaultValue((short)0);

        builder.HasOne(e => e.Kenshu)
            .WithMany(k => k.KenshuAttends)
            .HasForeignKey(e => e.KENSHUCD);

        builder.HasOne(e => e.User)
            .WithMany(u => u.KenshuAttends)
            .HasForeignKey(e => e.USERID);

        builder.HasIndex(e => new { e.KENSHUCD, e.USERID }).IsUnique().HasDatabaseName("UQ_ATTEND");
        builder.HasIndex(e => e.KENSHUCD).HasDatabaseName("IX_ATTEND_KENSHUCD");
        builder.HasIndex(e => e.USERID).HasDatabaseName("IX_ATTEND_USERID");
    }
}