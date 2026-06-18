using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Emp_KubunConfiguration : IEntityTypeConfiguration<T_Emp_Kubun>
{
    public void Configure(EntityTypeBuilder<T_Emp_Kubun> builder)
    {
        builder.ToTable("T_EMP_KUBUN");
        builder.HasKey(e => e.EmpKubun);
        builder.Property(e => e.EmpKubun).ValueGeneratedNever();
        builder.Property(e => e.KubunName).HasMaxLength(50).IsRequired();
    }
}