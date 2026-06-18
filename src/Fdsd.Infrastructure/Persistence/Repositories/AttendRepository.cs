using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class AttendRepository : IAttendRepository
{
    private readonly FdsdDbContext _db;

    public AttendRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<T_Kenshu_Attend> Query() => _db.T_KENSHU_ATTEND.AsQueryable();

    public async Task<List<T_Kenshu_Attend>> GetByKenshuCdAsync(int kenshuCd, CancellationToken ct = default)
    {
        return await _db.T_KENSHU_ATTEND.Where(x => x.KENSHUCD == kenshuCd).ToListAsync(ct);
    }

    public async Task<T_Kenshu_Attend?> GetByKeyAsync(int kenshuCd, int userId, CancellationToken ct = default)
    {
        return await _db.T_KENSHU_ATTEND.FirstOrDefaultAsync(x => x.KENSHUCD == kenshuCd && x.USERID == userId, ct);
    }

    public void Add(T_Kenshu_Attend attend) => _db.T_KENSHU_ATTEND.Add(attend);
    public void Update(T_Kenshu_Attend attend) => _db.T_KENSHU_ATTEND.Update(attend);
    public void Remove(T_Kenshu_Attend attend) => _db.T_KENSHU_ATTEND.Remove(attend);
    public void RemoveRange(IEnumerable<T_Kenshu_Attend> attends) => _db.T_KENSHU_ATTEND.RemoveRange(attends);
}