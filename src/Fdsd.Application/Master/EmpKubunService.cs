using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Master;

public class EmpKubunService
{
    private readonly IEmpKubunRepository _repo;
    private readonly IUnitOfWork _uow;

    public EmpKubunService(IEmpKubunRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public Task<List<T_Emp_Kubun>> GetAllAsync(CancellationToken ct) => _repo.GetAllAsync(ct);
    public Task<T_Emp_Kubun?> GetByIdAsync(short id, CancellationToken ct) => _repo.GetByIdAsync(id, ct);

    public async Task CreateAsync(T_Emp_Kubun entity, CancellationToken ct = default)
    {
        _repo.Add(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T_Emp_Kubun entity, CancellationToken ct = default)
    {
        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(short id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity == null) throw new NotFoundException(nameof(T_Emp_Kubun), id);
        _repo.Remove(entity);
        await _uow.SaveChangesAsync(ct);
    }
}