using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class LeaveOfAbsenceRepository : ILeaveOfAbsenceRepository
{
    private readonly FdsdDbContext _db;
    public LeaveOfAbsenceRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Leave_Of_Absence> Query() => _db.T_LEAVE_OF_ABSENCE.AsQueryable();

    public async Task<T_Leave_Of_Absence?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.T_LEAVE_OF_ABSENCE.FindAsync([id], ct);
    }

    public void Add(T_Leave_Of_Absence leave) => _db.T_LEAVE_OF_ABSENCE.Add(leave);
    public void Update(T_Leave_Of_Absence leave) => _db.T_LEAVE_OF_ABSENCE.Update(leave);
    public void Remove(T_Leave_Of_Absence leave) => _db.T_LEAVE_OF_ABSENCE.Remove(leave);
}