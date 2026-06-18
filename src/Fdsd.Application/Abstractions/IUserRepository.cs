using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IUserRepository
{
    IQueryable<M_User> Query();
    Task<M_User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<M_User?> GetByEmpUserNmAsync(string empUserNm, CancellationToken ct = default);
    void Add(M_User user);
    void Update(M_User user);
}