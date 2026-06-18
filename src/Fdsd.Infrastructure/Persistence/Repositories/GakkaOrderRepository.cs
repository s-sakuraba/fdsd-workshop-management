using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class GakkaOrderRepository : IGakkaOrderRepository
{
    private readonly FdsdDbContext _db;
    public GakkaOrderRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Gakka_Order> Query() => _db.T_GAKKA_ORDER.AsQueryable();

    public async Task<List<T_Gakka_Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.T_GAKKA_ORDER.ToListAsync(ct);
    }

    public void Add(T_Gakka_Order order) => _db.T_GAKKA_ORDER.Add(order);
    public void Update(T_Gakka_Order order) => _db.T_GAKKA_ORDER.Update(order);
    public void Remove(T_Gakka_Order order) => _db.T_GAKKA_ORDER.Remove(order);
}