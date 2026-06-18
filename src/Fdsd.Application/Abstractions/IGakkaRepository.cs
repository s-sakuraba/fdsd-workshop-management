using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IGakkaRepository
{
    IQueryable<M_Gakka> Query();
    Task<M_Gakka?> GetByIdAsync(short id, CancellationToken ct = default);
    Task<List<M_Gakka>> GetAllOrderedAsync(CancellationToken ct = default);
    void Add(M_Gakka gakka);
    void Update(M_Gakka gakka);
    void Remove(M_Gakka gakka);
}