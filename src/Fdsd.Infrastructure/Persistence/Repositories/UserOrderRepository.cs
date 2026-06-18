using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class UserOrderRepository : IUserOrderRepository
{
    private readonly FdsdDbContext _db;
    public UserOrderRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_User_Order> Query() => _db.T_USER_ORDER.AsQueryable();

    public async Task<List<T_User_Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.T_USER_ORDER.ToListAsync(ct);
    }

    public void Add(T_User_Order order) => _db.T_USER_ORDER.Add(order);
    public void Update(T_User_Order order) => _db.T_USER_ORDER.Update(order);
}