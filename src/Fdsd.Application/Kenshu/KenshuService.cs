using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;
using Fdsd.Domain.Enums;
using Fdsd.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Application.Kenshu;

public class KenshuService
{
    private readonly IKenshuRepository _kenshuRepo;
    private readonly IKenshuGakkaRepository _gakkaRepo;
    private readonly IAttendRepository _attendRepo;
    private readonly IGakkaChangeRepository _gakkaChangeRepo;
    private readonly IUserRepository _userRepo;
    private readonly IGakkaRepository _gakkaRepo2;
    private readonly IKCodeRepository _kCodeRepo;
    private readonly IKenshuStyleRepository _styleRepo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public KenshuService(
        IKenshuRepository kenshuRepo,
        IKenshuGakkaRepository gakkaRepo,
        IAttendRepository attendRepo,
        IGakkaChangeRepository gakkaChangeRepo,
        IUserRepository userRepo,
        IGakkaRepository gakkaRepo2,
        IKCodeRepository kCodeRepo,
        IKenshuStyleRepository styleRepo,
        IUnitOfWork uow,
        IClock clock)
    {
        _kenshuRepo = kenshuRepo;
        _gakkaRepo = gakkaRepo;
        _attendRepo = attendRepo;
        _gakkaChangeRepo = gakkaChangeRepo;
        _userRepo = userRepo;
        _gakkaRepo2 = gakkaRepo2;
        _kCodeRepo = kCodeRepo;
        _styleRepo = styleRepo;
        _uow = uow;
        _clock = clock;
    }

    public async Task<List<KenshuListItemDto>> SearchAsync(SearchConditionDto condition, CancellationToken ct = default)
    {
        var query = _kenshuRepo.Query()
            .Include(x => x.KenshuStyle)
            .AsQueryable();

        if (condition.StartDate.HasValue || condition.EndDate.HasValue)
        {
            var from = condition.StartDate ?? DateTime.MinValue;
            var to = condition.EndDate ?? DomainRules.MaxDate;
            query = query.Where(x =>
                x.KENSHUDATE <= to &&
                (x.ENDDATE ?? x.KENSHUDATE) >= from);
        }

        if (condition.FdsdCds?.Length == 1)
        {
            query = query.Where(x => x.FDSDCD == condition.FdsdCds[0]!.Value);
        }

        // No FdsdCds or both selected = no filter
        if (condition.GakkaCds?.Length > 0)
        {
            var gakkaKenshuCds = await _gakkaRepo.Query()
                .Where(x => condition.GakkaCds.Contains(x.GAKKACD))
                .Select(x => x.KENSHUCD)
                .Distinct()
                .ToListAsync(ct);
            query = query.Where(x => gakkaKenshuCds.Contains(x.KENSHUCD));
        }

        var list = await query
            .OrderBy(x => x.KENSHUDATE)
            .Select(x => new KenshuListItemDto(
                x.KENSHUCD,
                x.KENSHUNAME,
                x.KENSHUDATE,
                x.ENDDATE,
                x.FDSDCD == 1 ? "SD" : "FD",
                x.SHUSAINAME,
                x.KenshuStyle.NAME
            ))
            .ToListAsync(ct);

        return list;
    }

    public async Task<KenshuDetailDto?> GetDetailAsync(int id, CancellationToken ct = default)
    {
        var kenshu = await _kenshuRepo.GetByIdAsync(id, ct);
        if (kenshu == null) return null;

        var fdsdCodes = await _kCodeRepo.GetByCodeNoAsync(101, ct);
        var shusaiCodes = await _kCodeRepo.GetByCodeNoAsync(102, ct);

        return new KenshuDetailDto(
            kenshu.KENSHUCD,
            kenshu.KENSHUNAME,
            kenshu.KENSHUDATE,
            kenshu.ENDDATE,
            kenshu.FDSDCD,
            kenshu.SHUSAICD,
            kenshu.SHUSAINAME,
            kenshu.KSTYLECD,
            kenshu.KenshuStyle.NAME,
            kenshu.KenshuPlace,
            kenshu.INFODOCU,
            kenshu.KenshuDocuments.Select(d => new KenshuDocumentDto(d.ID, d.DOCUMENTNAME, d.DOCUMENTDIR)).ToList(),
            kenshu.KenshuGakkas.Select(g => g.GAKKACD).ToList(),
            kenshu.KenshuGakkas.Select(g => g.Gakka.GAKKANAME).ToList()
        );
    }

    public async Task<int> CreateAsync(string kenshuName, DateTime kenshuDate, DateTime? endDate,
        short fdsdCd, short shusaiCd, string shusaiName, short styleCd, string? kenshuPlace,
        short[] gakkaCds, string? infoDocu, int loginUserId, CancellationToken ct = default)
    {
        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var kenshu = new T_Kenshu
            {
                KENSHUNAME = kenshuName,
                KENSHUDATE = kenshuDate,
                ENDDATE = endDate,
                FDSDCD = fdsdCd,
                SHUSAICD = shusaiCd,
                SHUSAINAME = shusaiName,
                KSTYLECD = styleCd,
                KenshuPlace = kenshuPlace,
                INFODOCU = infoDocu,
                INSERTBI = _clock.Now,
                INSERTUSERID = loginUserId
            };
            _kenshuRepo.Add(kenshu);
            await _uow.SaveChangesAsync(ct);

            foreach (var gCd in gakkaCds)
            {
                _gakkaRepo.Add(new T_Kenshu_Gakka
                {
                    KENSHUCD = kenshu.KENSHUCD,
                    GAKKACD = gCd,
                    UPDATEUSERID = loginUserId,
                    UPDATEBI = _clock.Now
                });
            }

            // Initialize attend records
            var effectiveEnd = endDate ?? kenshuDate;
            var users = await _gakkaChangeRepo.Query()
                .Where(x => gakkaCds.Contains(x.GAKKACD)
                    && x.DateOfArrival <= effectiveEnd
                    && (x.DateOfDeparture == null || x.DateOfDeparture >= kenshuDate))
                .Select(x => x.USERID)
                .Distinct()
                .ToListAsync(ct);

            foreach (var uid in users)
            {
                _attendRepo.Add(new T_Kenshu_Attend
                {
                    KENSHUCD = kenshu.KENSHUCD,
                    USERID = uid,
                    ATTEND = (short)AttendStatus.Absent,
                    UPDATEUSERID = loginUserId,
                    UPDATEBI = _clock.Now
                });
            }

            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return kenshu.KENSHUCD;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateAsync(int kenshuCd, string kenshuName, DateTime kenshuDate, DateTime? endDate,
        short fdsdCd, short shusaiCd, string shusaiName, short styleCd, string? kenshuPlace,
        short[] gakkaCds, string? infoDocu, int loginUserId, CancellationToken ct = default)
    {
        var kenshu = await _kenshuRepo.Query()
            .Include(x => x.KenshuGakkas)
            .FirstOrDefaultAsync(x => x.KENSHUCD == kenshuCd, ct);

        if (kenshu == null) throw new NotFoundException(nameof(T_Kenshu), kenshuCd);

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            kenshu.KENSHUNAME = kenshuName;
            kenshu.KENSHUDATE = kenshuDate;
            kenshu.ENDDATE = endDate;
            kenshu.FDSDCD = fdsdCd;
            kenshu.SHUSAICD = shusaiCd;
            kenshu.SHUSAINAME = shusaiName;
            kenshu.KSTYLECD = styleCd;
            kenshu.KenshuPlace = kenshuPlace;
            kenshu.INFODOCU = infoDocu;
            kenshu.UPDATEUSERID = loginUserId;
            kenshu.UPDATEBI = _clock.Now;
            _kenshuRepo.Update(kenshu);

            await _uow.SaveChangesAsync(ct);

            // Update gakka
            var existingGakkas = await _gakkaRepo.GetByKenshuCdAsync(kenshuCd, ct);
            var toRemove = existingGakkas.Where(g => !gakkaCds.Contains(g.GAKKACD)).ToList();
            _gakkaRepo.RemoveRange(toRemove);

            var existingCds = existingGakkas.Select(g => g.GAKKACD).ToHashSet();
            foreach (var gCd in gakkaCds)
            {
                if (!existingCds.Contains(gCd))
                {
                    _gakkaRepo.Add(new T_Kenshu_Gakka
                    {
                        KENSHUCD = kenshuCd,
                        GAKKACD = gCd,
                        UPDATEUSERID = loginUserId,
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

    public async Task DeleteAsync(int kenshuCd, CancellationToken ct = default)
    {
        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var attends = await _attendRepo.Query().Where(x => x.KENSHUCD == kenshuCd).ToListAsync(ct);
            _attendRepo.RemoveRange(attends);

            var gakkas = await _gakkaRepo.GetByKenshuCdAsync(kenshuCd, ct);
            _gakkaRepo.RemoveRange(gakkas);

            var kenshu = await _kenshuRepo.Query().FirstOrDefaultAsync(x => x.KENSHUCD == kenshuCd, ct);
            if (kenshu != null)
                _kenshuRepo.Remove(kenshu);

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