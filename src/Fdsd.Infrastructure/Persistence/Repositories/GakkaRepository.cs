using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class GakkaRepository : IGakkaRepository
{
    private readonly FdsdDbContext _db;
    public GakkaRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<M_Gakka> Query() => _db.M_GAKKA.AsQueryable();

    public async Task<M_Gakka?> GetByIdAsync(short id, CancellationToken ct = default)
    {
        return await _db.M_GAKKA.FindAsync([id], ct);
    }

    public async Task<List<M_Gakka>> GetAllOrderedAsync(CancellationToken ct = default)
    {
        return await (from g in _db.M_GAKKA
                      join o in _db.T_GAKKA_ORDER on g.GAKKACD equals o.GAKKACD into gj
                      from o in gj.DefaultIfEmpty()
                      orderby o != null ? o.OrderNo : (short)999
                      select g).ToListAsync(ct);
    }

    public void Add(M_Gakka gakka) => _db.M_GAKKA.Add(gakka);
    public void Update(M_Gakka gakka) => _db.M_GAKKA.Update(gakka);
    public void Remove(M_Gakka gakka) => _db.M_GAKKA.Remove(gakka);
}