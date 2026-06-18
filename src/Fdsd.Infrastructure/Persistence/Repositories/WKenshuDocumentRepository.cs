using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class WKenshuDocumentRepository : IWKenshuDocumentRepository
{
    private readonly FdsdDbContext _db;
    public WKenshuDocumentRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<W_Kenshu_Document> Query() => _db.W_KENSHU_DOCUMENT.AsQueryable();

    public async Task<List<W_Kenshu_Document>> GetByUserAndKenshuAsync(int userId, int kenshuCd, CancellationToken ct = default)
    {
        return await _db.W_KENSHU_DOCUMENT.Where(x => x.UPDATEUSERID == userId && x.KENSHUCD == kenshuCd).ToListAsync(ct);
    }

    public void Add(W_Kenshu_Document doc) => _db.W_KENSHU_DOCUMENT.Add(doc);
    public void Remove(W_Kenshu_Document doc) => _db.W_KENSHU_DOCUMENT.Remove(doc);
    public void RemoveRange(IEnumerable<W_Kenshu_Document> docs) => _db.W_KENSHU_DOCUMENT.RemoveRange(docs);
}