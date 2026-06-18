using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FdsdDbContext _db;
    public UserRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<M_User> Query() => _db.M_USER.AsQueryable();

    public async Task<M_User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.M_USER.FindAsync([(short)id], ct);
    }

    public async Task<M_User?> GetByEmpUserNmAsync(string empUserNm, CancellationToken ct = default)
    {
        return await _db.M_USER.FirstOrDefaultAsync(x => x.EmpUserNm == empUserNm, ct);
    }

    public void Add(M_User user) => _db.M_USER.Add(user);
    public void Update(M_User user) => _db.M_USER.Update(user);
}