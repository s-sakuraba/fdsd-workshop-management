using System;
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

public class UserService
{
    private readonly IUserRepository _userRepo;
    private readonly IGakkaChangeRepository _gakkaChangeRepo;
    private readonly IUserOrderRepository _userOrderRepo;
    private readonly IGakkaOrderRepository _gakkaOrderRepo;
    private readonly IGakkaRepository _gakkaRepo;
    private readonly IAttendRepository _attendRepo;
    private readonly IKenshuRepository _kenshuRepo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public UserService(
        IUserRepository userRepo, IGakkaChangeRepository gakkaChangeRepo,
        IUserOrderRepository userOrderRepo, IGakkaOrderRepository gakkaOrderRepo,
        IGakkaRepository gakkaRepo, IAttendRepository attendRepo,
        IKenshuRepository kenshuRepo, IUnitOfWork uow, IClock clock)
    {
        _userRepo = userRepo;
        _gakkaChangeRepo = gakkaChangeRepo;
        _userOrderRepo = userOrderRepo;
        _gakkaOrderRepo = gakkaOrderRepo;
        _gakkaRepo = gakkaRepo;
        _attendRepo = attendRepo;
        _kenshuRepo = kenshuRepo;
        _uow = uow;
        _clock = clock;
    }

    public async Task<List<UserDto>> GetActiveUsersAsync(CancellationToken ct = default)
    {
        var now = _clock.Now;
        return await (from u in _userRepo.Query()
                      join gc in _gakkaChangeRepo.Query() on u.USERID equals gc.USERID
                      join g in _gakkaRepo.Query() on gc.GAKKACD equals g.GAKKACD
                      join go in _gakkaOrderRepo.Query() on gc.GAKKACD equals go.GAKKACD into goj
                      from go in goj.DefaultIfEmpty()
                      where u.ZaisyokuKbn == 1
                         && gc.DateOfArrival <= now
                         && (gc.DateOfDeparture == null || gc.DateOfDeparture >= now)
                      orderby go != null ? go.OrderNo : (short)999, u.EmpName
                      select new UserDto(
                          u.USERID, u.EmpCd, u.EmpName, u.EmpUserNm,
                          (short)gc.GAKKACD, g.GAKKANAME,
                          u.NyusyaDate, u.TaisyaDate,
                          u.FDSDCD, u.User_Role, u.ZaisyokuKbn, u.EmpKubun
                      )).Distinct().ToListAsync(ct);
    }

    public async Task<M_User?> GetByEmpUserNmAsync(string empUserNm, CancellationToken ct = default)
    {
        return await _userRepo.GetByEmpUserNmAsync(empUserNm, ct);
    }

    public async Task<bool> ExistsAsync(string empUserNm, CancellationToken ct = default)
    {
        return await _userRepo.Query().AnyAsync(x => x.EmpUserNm == empUserNm, ct);
    }

    public async Task<short> GetNextEmpCdAsync(CancellationToken ct = default)
    {
        var max = await _userRepo.Query().MaxAsync(x => (short?)x.EmpCd, ct) ?? 0;
        return (short)(max + 1);
    }

    public async Task<bool> IsAdministratorAsync(string empUserNm, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByEmpUserNmAsync(empUserNm, ct);
        return user?.User_Role == 1;
    }

    public async Task<int> CreateAsync(string empName, string empUserNm, short gakkaCd,
        DateTime nyusyaDate, byte? empKubun, short empCd, CancellationToken ct = default)
    {
        var gakka = await _gakkaRepo.GetByIdAsync(gakkaCd, ct);
        var fdsdCd = gakka?.FDSDCD ?? 1;
        var corpCd = fdsdCd == 2 ? (short)2 : (short)1;

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var user = new M_User
            {
                EmpCd = empCd,
                EmpName = empName,
                EmpUserNm = empUserNm,
                GAKKACD = gakkaCd,
                NyusyaDate = nyusyaDate,
                TaisyaDate = DomainRules.MaxDate,
                CorpCd = corpCd,
                FDSDCD = fdsdCd,
                ZaisyokuKbn = 1,
                EmpKubun = empKubun,
                User_Role = 0
            };
            _userRepo.Add(user);
            await _uow.SaveChangesAsync(ct);

            _userOrderRepo.Add(new T_User_Order
            {
                USERID = user.USERID,
                CorpCd = corpCd,
                EmpCd = empCd,
                EmpName = empName,
                GAKKACD = gakkaCd,
                GAKKANAME = gakka?.GAKKANAME ?? "",
                OrderNo = 0
            });

            _gakkaChangeRepo.Add(new T_Gakka_Change
            {
                USERID = user.USERID,
                GAKKACD = gakkaCd,
                DateOfArrival = nyusyaDate,
                DateOfDeparture = null
            });

            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return user.USERID;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateAsync(int userId, string empName, short gakkaCd, byte? empKubun, short empCd, string empUserNm, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct);
        if (user == null) throw new NotFoundException(nameof(M_User), userId);

        var gakka = await _gakkaRepo.GetByIdAsync(gakkaCd, ct);
        var fdsdCd = gakka?.FDSDCD ?? 1;

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            user.EmpName = empName;
            user.EmpCd = empCd;
            user.EmpUserNm = empUserNm;
            user.GAKKACD = gakkaCd;
            user.FDSDCD = fdsdCd;
            user.EmpKubun = empKubun;
            _userRepo.Update(user);
            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task DeleteAsync(int userId, DateTime taisyaDate, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct);
        if (user == null) throw new NotFoundException(nameof(M_User), userId);

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            user.ZaisyokuKbn = 0;
            user.TaisyaDate = taisyaDate;
            _userRepo.Update(user);

            var changes = await _gakkaChangeRepo.Query()
                .Where(x => x.USERID == userId && x.DateOfDeparture == null)
                .ToListAsync(ct);
            foreach (var c in changes)
            {
                c.DateOfDeparture = taisyaDate;
                _gakkaChangeRepo.Update(c);
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