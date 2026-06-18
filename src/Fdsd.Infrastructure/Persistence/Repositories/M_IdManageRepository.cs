using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class M_IdManageRepository : IM_IdManageRepository
{
    private readonly FdsdDbContext _db;
    public M_IdManageRepository(FdsdDbContext db) { _db = db; }

    public async Task<int> GetNextIdAsync(string tableName, CancellationToken ct = default)
    {
        var record = await _db.M_IDMANAGE.FindAsync([tableName], ct);
        if (record == null) return 1;
        var newId = record.ID + 1;
        record.ID = newId;
        await _db.SaveChangesAsync(ct);
        return newId;
    }
}