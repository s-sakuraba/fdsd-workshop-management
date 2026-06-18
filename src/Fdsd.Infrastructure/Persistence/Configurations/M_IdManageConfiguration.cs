using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class M_IdManageConfiguration : IEntityTypeConfiguration<M_IdManage>
{
    public void Configure(EntityTypeBuilder<M_IdManage> builder)
    {
        builder.ToTable("M_IDMANAGE");
        builder.HasKey(e => e.TABLENAME);
        builder.Property(e => e.TABLENAME).HasMaxLength(50);
    }
}