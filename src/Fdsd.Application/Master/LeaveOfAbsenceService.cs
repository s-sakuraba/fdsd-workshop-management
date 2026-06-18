using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Application.Master;

public class LeaveOfAbsenceService
{
    private readonly ILeaveOfAbsenceRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public LeaveOfAbsenceService(ILeaveOfAbsenceRepository repo, IUserRepository userRepo, IUnitOfWork uow)
    {
        _repo = repo; _userRepo = userRepo; _uow = uow;
    }

    public async Task<List<LeaveOfAbsenceDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await (from l in _repo.Query()
                      join u in _userRepo.Query() on l.USERID equals u.USERID
                      orderby l.StartDate descending
                      select new LeaveOfAbsenceDto(l.ID, l.USERID, u.EmpName, l.StartDate, l.EndDate))
                      .ToListAsync(ct);
    }

    public Task<T_Leave_Of_Absence?> GetByIdAsync(int id, CancellationToken ct) => _repo.GetByIdAsync(id, ct);

    public async Task AddAsync(T_Leave_Of_Absence entity, CancellationToken ct = default)
    {
        _repo.Add(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T_Leave_Of_Absence entity, CancellationToken ct = default)
    {
        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(T_Leave_Of_Absence entity, CancellationToken ct = default)
    {
        _repo.Remove(entity);
        await _uow.SaveChangesAsync(ct);
    }
}