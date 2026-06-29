using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;
using Fdsd.Domain.Enums;
using Fdsd.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Application.Report;

public class ReportService
{
    private readonly IKenshuRepository _kenshuRepo;
    private readonly IKenshuGakkaRepository _gakkaRepo;
    private readonly IAttendRepository _attendRepo;
    private readonly IGakkaChangeRepository _gakkaChangeRepo;
    private readonly IUserRepository _userRepo;
    private readonly IGakkaRepository _gakkaRepo2;
    private readonly IGakkaOrderRepository _gakkaOrderRepo;
    private readonly IUserOrderRepository _userOrderRepo;
    private readonly IKCodeRepository _kCodeRepo;
    private readonly IKenshuStyleRepository _styleRepo;
    private readonly ILeaveOfAbsenceRepository _leaveRepo;
    private readonly IEmpKubunRepository _empKubunRepo;
    private readonly IExcelReportWriter _writer;

    public ReportService(
        IKenshuRepository kenshuRepo, IKenshuGakkaRepository gakkaRepo,
        IAttendRepository attendRepo, IGakkaChangeRepository gakkaChangeRepo,
        IUserRepository userRepo, IGakkaRepository gakkaRepo2,
        IGakkaOrderRepository gakkaOrderRepo, IUserOrderRepository userOrderRepo,
        IKCodeRepository kCodeRepo, IKenshuStyleRepository styleRepo,
        ILeaveOfAbsenceRepository leaveRepo, IEmpKubunRepository empKubunRepo,
        IExcelReportWriter writer)
    {
        _kenshuRepo = kenshuRepo; _gakkaRepo = gakkaRepo;
        _attendRepo = attendRepo; _gakkaChangeRepo = gakkaChangeRepo;
        _userRepo = userRepo; _gakkaRepo2 = gakkaRepo2;
        _gakkaOrderRepo = gakkaOrderRepo; _userOrderRepo = userOrderRepo;
        _kCodeRepo = kCodeRepo; _styleRepo = styleRepo;
        _leaveRepo = leaveRepo; _empKubunRepo = empKubunRepo;
        _writer = writer;
    }

    public async Task<string> GenerateP001Async(string tempFilePath, DateTime? startDate, DateTime? endDate,
        short?[]? fdsdCds, short[] gakkaCds, CancellationToken ct = default)
    {
        var searchFrom = startDate ?? DateTime.MinValue;
        var searchTo = endDate ?? DomainRules.MaxDate;

        var kenshuQuery = _kenshuRepo.Query().Include(x => x.KenshuStyle).AsQueryable();
        kenshuQuery = kenshuQuery.Where(x => x.KENSHUDATE <= searchTo && (x.ENDDATE ?? x.KENSHUDATE) >= searchFrom);
        if (fdsdCds?.Length == 1) kenshuQuery = kenshuQuery.Where(x => x.FDSDCD == fdsdCds[0]!.Value);

        var trainingList = await kenshuQuery.OrderBy(x => x.KENSHUDATE).ToListAsync(ct);

        var trainingColumns = trainingList.Select(t => new P001TrainingColumn(
            t.KENSHUNAME, t.KENSHUDATE, t.SHUSAINAME,
            t.FDSDCD == 1 ? "SD" : "FD",
            t.KenshuStyle.NAME
        )).ToList();

        var users = await (from u in _userRepo.Query()
                           where u.ZaisyokuKbn == 1
                           select u).ToListAsync(ct);

        var userIds = users.Select(u => u.USERID).ToHashSet();

        var departments = new List<P001DepartmentData>();

        foreach (var gCd in gakkaCds)
        {
            var gakka = await _gakkaRepo2.GetByIdAsync(gCd, ct);
            if (gakka == null) continue;
            var deptName = gakka.GAKKANAME;

            var gakkaChanges = await (from gc in _gakkaChangeRepo.Query()
                                      join uo in _userOrderRepo.Query() on gc.USERID equals uo.USERID into uoj
                                      from uo in uoj.DefaultIfEmpty()
                                      where gc.GAKKACD == gCd
                                           && userIds.Contains(gc.USERID)
                                           && gc.DateOfArrival <= searchTo
                                           && (gc.DateOfDeparture == null || gc.DateOfDeparture >= searchFrom)
                                      orderby uo != null ? uo.OrderNo : (short)999
                                      select new { gc, OrderNo = uo != null ? uo.OrderNo : (short)999 })
                                      .Distinct().ToListAsync(ct);

            var deptUsers = (from gcData in gakkaChanges
                             join u in users on gcData.gc.USERID equals u.USERID
                             orderby gcData.OrderNo, u.EmpName
                             select new { u, gc = gcData.gc }).Distinct().ToList();

            var persons = new List<P001PersonRow>();
            foreach (var du in deptUsers)
            {
                var symbols = new List<string>();
                var fdsd = du.u.FDSDCD;
                int fdNonAttend = 0, sdNonAttend = 0;

                foreach (var t in trainingList)
                {
                    if (du.gc.DateOfArrival > (t.ENDDATE ?? t.KENSHUDATE) ||
                        (du.gc.DateOfDeparture.HasValue && du.gc.DateOfDeparture < t.KENSHUDATE) ||
                        (du.u.TaisyaDate.HasValue && du.u.TaisyaDate.Value < t.KENSHUDATE))
                    {
                        symbols.Add("／");
                        continue;
                    }

                    var attend = await _attendRepo.Query()
                        .FirstOrDefaultAsync(a => a.KENSHUCD == t.KENSHUCD && a.USERID == du.u.USERID, ct);

                    var leaves = await _leaveRepo.Query()
                        .Where(l => l.USERID == du.u.USERID
                            && l.StartDate <= t.KENSHUDATE
                            && (l.EndDate == null || l.EndDate >= t.KENSHUDATE))
                        .ToListAsync(ct);

                    var symbol = DomainRules.GetAttendSymbol(
                        attend?.ATTEND, t.KENSHUDATE, du.u.NyusyaDate, du.u.TaisyaDate,
                        leaves.FirstOrDefault()?.StartDate, leaves.FirstOrDefault()?.EndDate);

                    symbols.Add(symbol);

                    if (symbol != "○" && symbol != "／")
                    {
                        if (t.FDSDCD == 1) sdNonAttend++;
                        if (t.FDSDCD == 2) fdNonAttend++;
                    }
                }

                string? bikou = null;
                if (fdsd == 2)
                {
                    if (fdNonAttend > 0 && sdNonAttend > 0) bikou = "3";
                    else if (fdNonAttend > 0) bikou = "2";
                    else if (sdNonAttend > 0) bikou = "1";
                }
                else if (fdsd == 1)
                {
                    if (sdNonAttend > 0) bikou = "1";
                }

                persons.Add(new P001PersonRow(du.u.EmpName, symbols, bikou));
            }
            departments.Add(new P001DepartmentData(deptName, persons));
        }

        var period = $"{searchFrom:yyyy/MM/dd} ～ {searchTo:yyyy/MM/dd}";
        return await _writer.CreateP001Async(tempFilePath, period, departments, trainingColumns);
    }

    public async Task<string> GenerateP002Async(string tempFilePath, DateTime? startDate, DateTime? endDate,
        short?[]? fdsdCds, short[] gakkaCds, CancellationToken ct = default)
    {
        var searchFrom = startDate ?? DateTime.MinValue;
        var searchTo = endDate ?? DomainRules.MaxDate;

        var kenshuQuery = _kenshuRepo.Query().AsQueryable();
        kenshuQuery = kenshuQuery.Where(x => x.KENSHUDATE <= searchTo && (x.ENDDATE ?? x.KENSHUDATE) >= searchFrom);
        if (fdsdCds?.Length == 1) kenshuQuery = kenshuQuery.Where(x => x.FDSDCD == fdsdCds[0]!.Value);
        var trainingList = await kenshuQuery.OrderBy(x => x.KENSHUDATE).ToListAsync(ct);

        var users = await (from u in _userRepo.Query()
                           where u.ZaisyokuKbn == 1
                           select u).ToListAsync(ct);

        var userIds = users.Select(u => u.USERID).ToHashSet();

        var rows = new List<P002Row>();
        var processedUsers = new HashSet<int>();

        foreach (var gCd in gakkaCds)
        {
            var gakka = await _gakkaRepo2.GetByIdAsync(gCd, ct);
            if (gakka == null) continue;

            var gakkaChanges = await (from gc in _gakkaChangeRepo.Query()
                                      join uo in _userOrderRepo.Query() on gc.USERID equals uo.USERID into uoj
                                      from uo in uoj.DefaultIfEmpty()
                                      where gc.GAKKACD == gCd
                                           && userIds.Contains(gc.USERID)
                                           && gc.DateOfArrival <= searchTo
                                           && (gc.DateOfDeparture == null || gc.DateOfDeparture >= searchFrom)
                                      orderby uo != null ? uo.OrderNo : (short)999
                                      select new { gc.USERID, OrderNo = uo != null ? uo.OrderNo : (short)999 })
                                      .Distinct().ToListAsync(ct);

            var deptUsers = (from gcData in gakkaChanges
                             join u in users on gcData.USERID equals u.USERID
                             orderby gcData.OrderNo, u.EmpName
                             select u).Distinct().ToList();

            foreach (var user in deptUsers)
            {
                if (processedUsers.Contains(user.USERID)) continue;
                processedUsers.Add(user.USERID);

                bool hasAttend = false;
                foreach (var t in trainingList)
                {
                    if ((user.TaisyaDate.HasValue && user.TaisyaDate.Value < t.KENSHUDATE) ||
                        (user.NyusyaDate.HasValue && user.NyusyaDate.Value > (t.ENDDATE ?? t.KENSHUDATE)))
                        continue;

                    var leaves = await _leaveRepo.Query()
                        .Where(l => l.USERID == user.USERID
                            && l.StartDate <= t.KENSHUDATE
                            && (l.EndDate == null || l.EndDate >= t.KENSHUDATE))
                        .ToListAsync(ct);

                    if (leaves.Any()) continue;

                    var attend = await _attendRepo.Query()
                        .FirstOrDefaultAsync(a => a.KENSHUCD == t.KENSHUCD && a.USERID == user.USERID, ct);

                    if (attend?.ATTEND == (short)AttendStatus.Present)
                    {
                        hasAttend = true;
                        break;
                    }
                }

                if (!hasAttend)
                {
                    var fdsdName = user.FDSDCD == 1 ? "SD" : "FD";
                    rows.Add(new P002Row(user.EmpName, fdsdName, gakka.GAKKANAME));
                }
            }
        }

        var period = $"{searchFrom:yyyy/MM/dd} ～ {searchTo:yyyy/MM/dd}";
        return await _writer.CreateP002Async(tempFilePath, period, rows);
    }

    public async Task<string> GenerateP003Async(string templatePath, string tempFilePath,
        DateTime? startDate, DateTime? endDate, short?[]? fdsdCds, short[] gakkaCds, CancellationToken ct = default)
    {
        var searchFrom = startDate ?? DateTime.MinValue;
        var searchTo = endDate ?? DomainRules.MaxDate;

        var kenshuQuery = _kenshuRepo.Query()
            .Include(x => x.KenshuStyle)
            .Include(x => x.KenshuGakkas).ThenInclude(x => x.Gakka)
            .Include(x => x.KenshuAttends).ThenInclude(x => x.User)
            .Include(x => x.KenshuDocuments)
            .AsQueryable();

        kenshuQuery = kenshuQuery.Where(x => x.KENSHUDATE <= searchTo && (x.ENDDATE ?? x.KENSHUDATE) >= searchFrom);
        if (fdsdCds?.Length == 1) kenshuQuery = kenshuQuery.Where(x => x.FDSDCD == fdsdCds[0]!.Value);

        var gakkaKenshuCds = await _gakkaRepo.Query()
            .Where(x => gakkaCds.Contains(x.GAKKACD))
            .Select(x => x.KENSHUCD)
            .Distinct()
            .ToListAsync(ct);

        var trainingList = await kenshuQuery
            .Where(x => gakkaKenshuCds.Contains(x.KENSHUCD))
            .OrderBy(x => x.KENSHUDATE)
            .ToListAsync(ct);

        var shusaiCodes = await _kCodeRepo.GetByCodeNoAsync(102, ct);
        var shusaiLookup = shusaiCodes.ToDictionary(x => x.CODE, x => x.RYAKUSHO);

        var rows = new List<P003Row>();
        foreach (var t in trainingList)
        {
            var fdsdName = t.FDSDCD == 1 ? "SD" : "FD";
            var shusaiDetail = shusaiLookup.GetValueOrDefault(t.SHUSAICD, "");
            var gakkaNames = string.Join(", ", t.KenshuGakkas.Select(g => g.Gakka.GAKKANAME));
            var docNames = string.Join(", ", t.KenshuDocuments.Select(d => d.DOCUMENTNAME));

            var presentUsers = t.KenshuAttends.Where(a => a.ATTEND == (short)AttendStatus.Present).ToList();
            var totalAttendees = presentUsers.Count;
            var staffCount = presentUsers.Count(a => a.User.EmpKubun == 1);
            var teacherCount = presentUsers.Count(a => a.User.EmpKubun == 2);
            var dispatchCount = presentUsers.Count(a => a.User.EmpKubun == 3);
            var otherCount = presentUsers.Count(a => a.User.EmpKubun != 1 && a.User.EmpKubun != 2 && a.User.EmpKubun != 3);

            rows.Add(new P003Row(
                fdsdName, t.KENSHUDATE, t.KENSHUNAME, t.SHUSAINAME,
                shusaiDetail, gakkaNames, t.INFODOCU, docNames,
                t.KenshuPlace, totalAttendees, staffCount, teacherCount, dispatchCount, otherCount));
        }

        var period = $"{searchFrom:yyyy/MM/dd} ～ {searchTo:yyyy/MM/dd}";
        return await _writer.CreateP003Async(templatePath, tempFilePath, period, rows);
    }
}