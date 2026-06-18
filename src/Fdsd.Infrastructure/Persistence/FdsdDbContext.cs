using Microsoft.EntityFrameworkCore;
using Fdsd.Domain.Entities;

namespace Fdsd.Infrastructure.Persistence;

public class FdsdDbContext : DbContext
{
    public FdsdDbContext(DbContextOptions<FdsdDbContext> options) : base(options) { }

    public DbSet<M_Gakka> M_GAKKA => Set<M_Gakka>();
    public DbSet<M_User> M_USER => Set<M_User>();
    public DbSet<M_IdManage> M_IDMANAGE => Set<M_IdManage>();
    public DbSet<K_Code> K_CODE => Set<K_Code>();
    public DbSet<T_Emp_Kubun> T_EMP_KUBUN => Set<T_Emp_Kubun>();
    public DbSet<T_Kenshu> T_KENSHU => Set<T_Kenshu>();
    public DbSet<T_Kenshu_Attend> T_KENSHU_ATTEND => Set<T_Kenshu_Attend>();
    public DbSet<T_Kenshu_Gakka> T_KENSHU_GAKKA => Set<T_Kenshu_Gakka>();
    public DbSet<T_Kenshu_Document> T_KENSHU_DOCUMENT => Set<T_Kenshu_Document>();
    public DbSet<W_Kenshu_Document> W_KENSHU_DOCUMENT => Set<W_Kenshu_Document>();
    public DbSet<T_Gakka_Change> T_GAKKA_CHANGE => Set<T_Gakka_Change>();
    public DbSet<T_Kenshu_Style> T_KENSHU_STYLE => Set<T_Kenshu_Style>();
    public DbSet<T_Gakka_Order> T_GAKKA_ORDER => Set<T_Gakka_Order>();
    public DbSet<T_User_Order> T_USER_ORDER => Set<T_User_Order>();
    public DbSet<T_Leave_Of_Absence> T_LEAVE_OF_ABSENCE => Set<T_Leave_Of_Absence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FdsdDbContext).Assembly);
    }
}