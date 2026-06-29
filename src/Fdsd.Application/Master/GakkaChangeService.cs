using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;
using Fdsd.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Application.Master;

public class GakkaChangeService
{
    private readonly IGakkaChangeRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IUserRepository _userRepo;
    private readonly IGakkaRepository _gakkaRepo;

    public GakkaChangeService(IGakkaChangeRepository repo, IUserRepository userRepo,
        IGakkaRepository gakkaRepo, IUnitOfWork uow)
    {
        _repo = repo;
        _userRepo = userRepo;
        _gakkaRepo = gakkaRepo;
        _uow = uow;
    }

    public async Task<List<GakkaChangeDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await (from gc in _repo.Query()
                      join u in _userRepo.Query() on gc.USERID equals u.USERID
                      join g in _gakkaRepo.Query() on gc.GAKKACD equals g.GAKKACD
                      orderby gc.GAKKACD
                      select new GakkaChangeDto(
                          gc.ID, gc.USERID, u.EmpName, gc.GAKKACD, g.GAKKANAME,
                          gc.DateOfArrival, gc.DateOfDeparture
                      )).ToListAsync(ct);
    }

    public async Task AddAsync(int userId, short gakkaCd, DateTime dateOfArrival, DateTime? dateOfDeparture, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct);
        if (user == null) throw new NotFoundException(nameof(M_User), userId);

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var prev = await _repo.Query()
                .Where(x => x.USERID == userId)
                .OrderByDescending(x => x.DateOfArrival)
                .FirstOrDefaultAsync(ct);

            if (prev != null && (prev.DateOfDeparture == null || prev.DateOfDeparture == DomainRules.MaxDate))
            {
                prev.DateOfDeparture = dateOfArrival.AddDays(-1);
                _repo.Update(prev);
            }

            _repo.Add(new T_Gakka_Change
            {
                USERID = (short)userId,
                GAKKACD = gakkaCd,
                DateOfArrival = dateOfArrival,
                DateOfDeparture = dateOfDeparture ?? DomainRules.MaxDate
            });

            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch { await tx.RollbackAsync(ct); throw; }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var target = await _repo.Query().FirstOrDefaultAsync(x => x.ID == id, ct);
        if (target == null) throw new NotFoundException(nameof(T_Gakka_Change), id);

        var count = await _repo.Query().CountAsync(x => x.USERID == target.USERID, ct);
        if (count < 2) throw new DomainException("Cannot delete last gakka change record for user");

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var prev = await _repo.Query()
                .Where(x => x.USERID == target.USERID && x.ID != id)
                .OrderByDescending(x => x.DateOfArrival)
                .FirstOrDefaultAsync(ct);

            if (prev != null)
                prev.DateOfDeparture = DomainRules.MaxDate;

            _repo.Remove(target);
            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch { await tx.RollbackAsync(ct); throw; }
    }
}