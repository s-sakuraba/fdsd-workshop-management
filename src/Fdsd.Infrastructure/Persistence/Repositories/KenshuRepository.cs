using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class KenshuRepository : IKenshuRepository
{
    private readonly FdsdDbContext _db;

    public KenshuRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Kenshu> Query() => _db.T_KENSHU.AsQueryable();

    public async Task<T_Kenshu?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.T_KENSHU
            .Include(x => x.KenshuStyle)
            .Include(x => x.KenshuGakkas).ThenInclude(x => x.Gakka)
            .Include(x => x.KenshuDocuments)
            .FirstOrDefaultAsync(x => x.KENSHUCD == id, ct);
    }

    public async Task<List<T_Kenshu>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        return await _db.T_KENSHU.Where(x => ids.Contains(x.KENSHUCD)).ToListAsync(ct);
    }

    public void Add(T_Kenshu kenshu) => _db.T_KENSHU.Add(kenshu);
    public void Update(T_Kenshu kenshu) => _db.T_KENSHU.Update(kenshu);
    public void Remove(T_Kenshu kenshu) => _db.T_KENSHU.Remove(kenshu);
}