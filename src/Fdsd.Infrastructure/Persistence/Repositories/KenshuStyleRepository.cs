using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class KenshuStyleRepository : IKenshuStyleRepository
{
    private readonly FdsdDbContext _db;
    public KenshuStyleRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Kenshu_Style> Query() => _db.T_KENSHU_STYLE.AsQueryable();

    public async Task<List<T_Kenshu_Style>> GetAllOrderedAsync(CancellationToken ct = default)
    {
        var list = await _db.T_KENSHU_STYLE.ToListAsync(ct);
        list.Sort((a, b) => a.SORT.CompareTo(b.SORT));
        return list;
    }

    public async Task<T_Kenshu_Style?> GetByIdAsync(short id, CancellationToken ct = default)
    {
        return await _db.T_KENSHU_STYLE.FindAsync([id], ct);
    }

    public void Add(T_Kenshu_Style style) => _db.T_KENSHU_STYLE.Add(style);
    public void Update(T_Kenshu_Style style) => _db.T_KENSHU_STYLE.Update(style);
    public void Remove(T_Kenshu_Style style) => _db.T_KENSHU_STYLE.Remove(style);
}