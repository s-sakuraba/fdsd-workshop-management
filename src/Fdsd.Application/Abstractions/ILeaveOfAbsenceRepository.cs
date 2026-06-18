using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface ILeaveOfAbsenceRepository
{
    IQueryable<T_Leave_Of_Absence> Query();
    Task<T_Leave_Of_Absence?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(T_Leave_Of_Absence leave);
    void Update(T_Leave_Of_Absence leave);
    void Remove(T_Leave_Of_Absence leave);
}