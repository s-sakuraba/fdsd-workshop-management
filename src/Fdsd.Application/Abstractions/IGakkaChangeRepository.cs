using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IGakkaChangeRepository
{
    IQueryable<T_Gakka_Change> Query();
    Task<List<T_Gakka_Change>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    void Add(T_Gakka_Change change);
    void Update(T_Gakka_Change change);
    void Remove(T_Gakka_Change change);
}