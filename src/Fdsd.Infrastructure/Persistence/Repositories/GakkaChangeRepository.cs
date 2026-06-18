using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class GakkaChangeRepository : IGakkaChangeRepository
{
    private readonly FdsdDbContext _db;
    public GakkaChangeRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Gakka_Change> Query() => _db.T_GAKKA_CHANGE.AsQueryable();

    public async Task<List<T_Gakka_Change>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _db.T_GAKKA_CHANGE.Where(x => x.USERID == userId).OrderBy(x => x.DateOfArrival).ToListAsync(ct);
    }

    public void Add(T_Gakka_Change change) => _db.T_GAKKA_CHANGE.Add(change);
    public void Update(T_Gakka_Change change) => _db.T_GAKKA_CHANGE.Update(change);
    public void Remove(T_Gakka_Change change) => _db.T_GAKKA_CHANGE.Remove(change);
}