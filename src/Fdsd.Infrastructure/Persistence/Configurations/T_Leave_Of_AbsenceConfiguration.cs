using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Leave_Of_AbsenceConfiguration : IEntityTypeConfiguration<T_Leave_Of_Absence>
{
    public void Configure(EntityTypeBuilder<T_Leave_Of_Absence> builder)
    {
        builder.ToTable("T_LEARVE_OF_ADSENCE");
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID).ValueGeneratedOnAdd();
        builder.Property(e => e.StartDate).IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(u => u.LeaveOfAbsences)
            .HasForeignKey(e => e.USERID);
    }
}