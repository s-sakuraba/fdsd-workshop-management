using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class KenshuDocumentRepository : IKenshuDocumentRepository
{
    private readonly FdsdDbContext _db;
    public KenshuDocumentRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Kenshu_Document> Query() => _db.T_KENSHU_DOCUMENT.AsQueryable();

    public async Task<List<T_Kenshu_Document>> GetByKenshuCdAsync(int kenshuCd, CancellationToken ct = default)
    {
        return await _db.T_KENSHU_DOCUMENT.Where(x => x.KENSHUCD == kenshuCd).ToListAsync(ct);
    }

    public void Add(T_Kenshu_Document doc) => _db.T_KENSHU_DOCUMENT.Add(doc);
    public void Remove(T_Kenshu_Document doc) => _db.T_KENSHU_DOCUMENT.Remove(doc);
    public void AddRange(IEnumerable<T_Kenshu_Document> docs) => _db.T_KENSHU_DOCUMENT.AddRange(docs);
    public void RemoveRange(IEnumerable<T_Kenshu_Document> docs) => _db.T_KENSHU_DOCUMENT.RemoveRange(docs);
}