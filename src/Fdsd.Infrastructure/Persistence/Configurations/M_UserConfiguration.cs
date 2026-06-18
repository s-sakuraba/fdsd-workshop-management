using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class M_UserConfiguration : IEntityTypeConfiguration<M_User>
{
    public void Configure(EntityTypeBuilder<M_User> builder)
    {
        builder.ToTable("M_USER");
        builder.HasKey(e => e.USERID);
        builder.Property(e => e.USERID).ValueGeneratedOnAdd();
        builder.Property(e => e.EmpName).HasMaxLength(20).IsRequired();
        builder.Property(e => e.EmpUserNm).HasMaxLength(20).IsRequired();
        builder.Property(e => e.NyusyaDate).HasColumnName("NyusyaYMD");
        builder.Property(e => e.TaisyaDate).HasColumnName("TaisyaYMD");
        builder.Property(e => e.ZaisyokuKbn).HasDefaultValue((short)1);
        builder.Property(e => e.User_Role).HasDefaultValue((byte)0);

        builder.HasOne(e => e.Gakka)
            .WithMany(g => g.Users)
            .HasForeignKey(e => e.GAKKACD);

        builder.HasOne(e => e.EmpKubunRef)
            .WithMany()
            .HasForeignKey(e => e.EmpKubun);

        builder.HasIndex(e => e.EmpUserNm).HasDatabaseName("IX_M_USER_EmpUserNm");
        builder.HasIndex(e => e.GAKKACD).HasDatabaseName("IX_M_USER_GAKKACD");
    }
}