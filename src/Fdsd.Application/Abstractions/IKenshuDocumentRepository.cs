using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IKenshuDocumentRepository
{
    IQueryable<T_Kenshu_Document> Query();
    Task<List<T_Kenshu_Document>> GetByKenshuCdAsync(int kenshuCd, CancellationToken ct = default);
    void Add(T_Kenshu_Document doc);
    void Remove(T_Kenshu_Document doc);
    void AddRange(IEnumerable<T_Kenshu_Document> docs);
    void RemoveRange(IEnumerable<T_Kenshu_Document> docs);
}