using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IUserOrderRepository
{
    IQueryable<T_User_Order> Query();
    Task<List<T_User_Order>> GetAllAsync(CancellationToken ct = default);
    void Add(T_User_Order order);
    void Update(T_User_Order order);
}