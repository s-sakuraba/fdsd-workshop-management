using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class EmpKubunRepository : IEmpKubunRepository
{
    private readonly FdsdDbContext _db;
    public EmpKubunRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Emp_Kubun> Query() => _db.T_EMP_KUBUN.AsQueryable();

    public async Task<List<T_Emp_Kubun>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.T_EMP_KUBUN.ToListAsync(ct);
    }

    public async Task<T_Emp_Kubun?> GetByIdAsync(short id, CancellationToken ct = default)
    {
        return await _db.T_EMP_KUBUN.FindAsync([(byte)id], ct);
    }

    public void Add(T_Emp_Kubun kubun) => _db.T_EMP_KUBUN.Add(kubun);
    public void Update(T_Emp_Kubun kubun) => _db.T_EMP_KUBUN.Update(kubun);
    public void Remove(T_Emp_Kubun kubun) => _db.T_EMP_KUBUN.Remove(kubun);
}