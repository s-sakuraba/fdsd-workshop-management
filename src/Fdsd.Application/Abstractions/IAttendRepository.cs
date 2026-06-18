using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IAttendRepository
{
    IQueryable<T_Kenshu_Attend> Query();
    Task<List<T_Kenshu_Attend>> GetByKenshuCdAsync(int kenshuCd, CancellationToken ct = default);
    Task<T_Kenshu_Attend?> GetByKeyAsync(int kenshuCd, int userId, CancellationToken ct = default);
    void Add(T_Kenshu_Attend attend);
    void Update(T_Kenshu_Attend attend);
    void Remove(T_Kenshu_Attend attend);
    void RemoveRange(IEnumerable<T_Kenshu_Attend> attends);
}