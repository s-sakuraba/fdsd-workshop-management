using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence.Configurations;

public class T_Gakka_ChangeConfiguration : IEntityTypeConfiguration<T_Gakka_Change>
{
    public void Configure(EntityTypeBuilder<T_Gakka_Change> builder)
    {
        builder.ToTable("T_GAKKA_CHANGE");
        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID).ValueGeneratedOnAdd();
        builder.Property(e => e.DateOfArrival).IsRequired();
        builder.Property(e => e.DateOfDeparture).HasColumnName("date_of_departure");

        builder.HasOne(e => e.User)
            .WithMany(u => u.GakkaChanges)
            .HasForeignKey(e => e.USERID);

        builder.HasOne(e => e.Gakka)
            .WithMany(g => g.GakkaChanges)
            .HasForeignKey(e => e.GAKKACD);

        builder.HasIndex(e => e.USERID).HasDatabaseName("IX_GAKKA_CHANGE_USERID");
        builder.HasIndex(e => e.GAKKACD).HasDatabaseName("IX_GAKKA_CHANGE_GAKKACD");
    }
}