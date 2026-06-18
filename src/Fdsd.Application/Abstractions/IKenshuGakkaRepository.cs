using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IKenshuGakkaRepository
{
    IQueryable<T_Kenshu_Gakka> Query();
    Task<List<T_Kenshu_Gakka>> GetByKenshuCdAsync(int kenshuCd, CancellationToken ct = default);
    void Add(T_Kenshu_Gakka gakka);
    void Remove(T_Kenshu_Gakka gakka);
    void AddRange(IEnumerable<T_Kenshu_Gakka> gakkas);
    void RemoveRange(IEnumerable<T_Kenshu_Gakka> gakkas);
}