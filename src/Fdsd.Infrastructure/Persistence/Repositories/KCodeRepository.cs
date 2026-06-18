using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Infrastructure.Persistence.Repositories;

public class KCodeRepository : IKCodeRepository
{
    private readonly FdsdDbContext _db;
    public KCodeRepository(FdsdDbContext db) { _db = db; }

    public IQueryable<K_Code> Query() => _db.K_CODE.AsQueryable();

    public async Task<List<K_Code>> GetByCodeNoAsync(short codeNo, CancellationToken ct = default)
    {
        return await _db.K_CODE.Where(x => x.CODENO == codeNo).OrderBy(x => x.SORT).ToListAsync(ct);
    }
}