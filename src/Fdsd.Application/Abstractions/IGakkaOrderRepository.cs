using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IGakkaOrderRepository
{
    IQueryable<T_Gakka_Order> Query();
    Task<List<T_Gakka_Order>> GetAllAsync(CancellationToken ct = default);
    void Add(T_Gakka_Order order);
    void Update(T_Gakka_Order order);
    void Remove(T_Gakka_Order order);
}