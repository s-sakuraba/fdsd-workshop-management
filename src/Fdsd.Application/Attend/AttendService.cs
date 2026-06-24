using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;
using Fdsd.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Application.Attend;

public class AttendService
{
    private readonly IAttendRepository _attendRepo;
    private readonly IKenshuRepository _kenshuRepo;
    private readonly IGakkaChangeRepository _gakkaChangeRepo;
    private readonly IUserRepository _userRepo;
    private readonly IGakkaRepository _gakkaRepo;
    private readonly IGakkaOrderRepository _gakkaOrderRepo;
    private readonly IUserOrderRepository _userOrderRepo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public AttendService(
        IAttendRepository attendRepo, IKenshuRepository kenshuRepo,
        IGakkaChangeRepository gakkaChangeRepo, IUserRepository userRepo,
        IGakkaRepository gakkaRepo, IGakkaOrderRepository gakkaOrderRepo,
        IUserOrderRepository userOrderRepo, IUnitOfWork uow, IClock clock)
    {
        _attendRepo = attendRepo;
        _kenshuRepo = kenshuRepo;
        _gakkaChangeRepo = gakkaChangeRepo;
        _userRepo = userRepo;
        _gakkaRepo = gakkaRepo;
        _gakkaOrderRepo = gakkaOrderRepo;
        _userOrderRepo = userOrderRepo;
        _uow = uow;
        _clock = clock;
    }

    public async Task<string> GetKenshuNameAsync(int kenshuCd, CancellationToken ct = default)
    {
        var kenshu = await _kenshuRepo.Query().FirstOrDefaultAsync(x => x.KENSHUCD == kenshuCd, ct);
        return kenshu?.KENSHUNAME ?? "";
    }

    public async Task<List<AttendItemDto>> GetAttendListAsync(int kenshuCd, CancellationToken ct = default)
    {
        var kenshu = await _kenshuRepo.Query().FirstOrDefaultAsync(x => x.KENSHUCD == kenshuCd, ct);
        if (kenshu == null) throw new NotFoundException(nameof(T_Kenshu), kenshuCd);

        var effectiveEnd = kenshu.ENDDATE ?? kenshu.KENSHUDATE;

        var gakkaOrderLookup = await _gakkaOrderRepo.Query().ToDictionaryAsync(g => (int)g.GAKKACD, g => (short?)g.OrderNo, ct);
        var userOrderLookup = await _userOrderRepo.Query().ToDictionaryAsync(u => u.USERID, u => (short?)u.OrderNo, ct);

        var attendLookup = await _attendRepo.Query()
            .Where(x => x.KENSHUCD == kenshuCd)
            .ToDictionaryAsync(x => x.USERID, x => x.ATTEND, ct);

        var result = new List<AttendItemDto>();

        // Get users belonging to target department during training period
        var users = await (from gc in _gakkaChangeRepo.Query()
                           join u in _userRepo.Query() on gc.USERID equals u.USERID
                           join g in _gakkaRepo.Query() on gc.GAKKACD equals g.GAKKACD
                           where gc.DateOfArrival <= effectiveEnd
                              && (gc.DateOfDeparture == null || gc.DateOfDeparture >= kenshu.KENSHUDATE)
                              && !(u.TaisyaDate.HasValue && u.TaisyaDate.Value <= kenshu.KENSHUDATE)
                              && u.ZaisyokuKbn == 1
                           select new
                           {
                               u.USERID,
                               u.EmpName,
                               gc.GAKKACD,
                               g.GAKKANAME
                           }).Distinct().ToListAsync(ct);

        foreach (var u in users)
        {
            var gakkaOrder = gakkaOrderLookup.GetValueOrDefault((int)u.GAKKACD) ?? 999;
            var userOrder = userOrderLookup.GetValueOrDefault(u.USERID) ?? 999;
            var attend = attendLookup.GetValueOrDefault(u.USERID);

            result.Add(new AttendItemDto(
                u.USERID,
                u.EmpName,
                u.GAKKACD,
                u.GAKKANAME,
                attend,
                gakkaOrder,
                userOrder
            ));
        }

        return result.OrderBy(x => x.GakkaOrderNo).ThenBy(x => x.UserOrderNo).ToList();
    }

    public async Task SaveAttendsAsync(int kenshuCd, List<(int userId, short attend)> attendData, int loginUserId, CancellationToken ct = default)
    {
        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            foreach (var (userId, attend) in attendData)
            {
                var existing = await _attendRepo.GetByKeyAsync(kenshuCd, userId, ct);
                if (existing != null)
                {
                    existing.ATTEND = attend;
                    existing.UPDATEUSERID = (short)loginUserId;
                    existing.UPDATEBI = _clock.Now;
                    _attendRepo.Update(existing);
                }
                else
                {
                    _attendRepo.Add(new T_Kenshu_Attend
                    {
                        KENSHUCD = (short)kenshuCd,
                        USERID = (short)userId,
                        ATTEND = attend,
                        UPDATEUSERID = (short)loginUserId,
                        UPDATEBI = _clock.Now
                    });
                }
            }
            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}