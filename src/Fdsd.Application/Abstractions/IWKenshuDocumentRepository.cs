using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IWKenshuDocumentRepository
{
    IQueryable<W_Kenshu_Document> Query();
    Task<List<W_Kenshu_Document>> GetByUserAndKenshuAsync(int userId, int kenshuCd, CancellationToken ct = default);
    void Add(W_Kenshu_Document doc);
    void Remove(W_Kenshu_Document doc);
    void RemoveRange(IEnumerable<W_Kenshu_Document> docs);
}