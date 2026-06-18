using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IEmpKubunRepository
{
    IQueryable<T_Emp_Kubun> Query();
    Task<List<T_Emp_Kubun>> GetAllAsync(CancellationToken ct = default);
    Task<T_Emp_Kubun?> GetByIdAsync(short id, CancellationToken ct = default);
    void Add(T_Emp_Kubun kubun);
    void Update(T_Emp_Kubun kubun);
    void Remove(T_Emp_Kubun kubun);
}