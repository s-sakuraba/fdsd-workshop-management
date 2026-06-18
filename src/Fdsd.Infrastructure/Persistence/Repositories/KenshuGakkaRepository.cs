using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class KenshuGakkaRepository : IKenshuGakkaRepository
{
    private readonly FdsdDbContext _db;
    public KenshuGakkaRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Kenshu_Gakka> Query() => _db.T_KENSHU_GAKKA.AsQueryable();

    public async Task<List<T_Kenshu_Gakka>> GetByKenshuCdAsync(int kenshuCd, CancellationToken ct = default)
    {
        return await _db.T_KENSHU_GAKKA.Where(x => x.KENSHUCD == kenshuCd).ToListAsync(ct);
    }

    public void Add(T_Kenshu_Gakka gakka) => _db.T_KENSHU_GAKKA.Add(gakka);
    public void Remove(T_Kenshu_Gakka gakka) => _db.T_KENSHU_GAKKA.Remove(gakka);
    public void AddRange(IEnumerable<T_Kenshu_Gakka> gakkas) => _db.T_KENSHU_GAKKA.AddRange(gakkas);
    public void RemoveRange(IEnumerable<T_Kenshu_Gakka> gakkas) => _db.T_KENSHU_GAKKA.RemoveRange(gakkas);
}