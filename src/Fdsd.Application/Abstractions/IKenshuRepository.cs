using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IKenshuRepository
{
    IQueryable<T_Kenshu> Query();
    Task<T_Kenshu?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<T_Kenshu>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
    void Add(T_Kenshu kenshu);
    void Update(T_Kenshu kenshu);
    void Remove(T_Kenshu kenshu);
}