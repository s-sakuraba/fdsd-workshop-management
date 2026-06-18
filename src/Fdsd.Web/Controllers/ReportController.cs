using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Report;

namespace Fdsd.Web.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly ReportService _reportService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

    public ReportController(ReportService reportService, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        _reportService = reportService;
        _config = config;
    }

    public IActionResult Index() => View();

    private (DateTime? startDate, DateTime? endDate, short?[]? fdsdCds, short[] gakkaCds) ReadConditions()
    {
        short?[]? fdsdCds = null;
        if (TempData["FDSDCondition"] is string fdsdJson)
            fdsdCds = System.Text.Json.JsonSerializer.Deserialize<short?[]>(fdsdJson);

        short[] gakkaCds = [];
        if (TempData["GAKKACondition"] is string gakkaJson)
            gakkaCds = System.Text.Json.JsonSerializer.Deserialize<short[]>(gakkaJson) ?? [];

        DateTime? startDate = null;
        if (TempData["StartDate"] is string sDate && DateTime.TryParseExact(sDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var sd))
            startDate = sd;

        DateTime? endDate = null;
        if (TempData["EndDate"] is string eDate && DateTime.TryParseExact(eDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var ed))
            endDate = ed;

        TempData.Keep();
        return (startDate, endDate, fdsdCds, gakkaCds);
    }

    public async Task<IActionResult> Report_P001(CancellationToken ct = default)
    {
        var tempFile = _config["FilePaths:P001Temp"] ?? @"C:\Temp\研修会出席状況表_temp.xlsx";
        var (startDate, endDate, fdsdCds, gakkaCds) = ReadConditions();
        await _reportService.GenerateP001Async(tempFile, startDate, endDate, fdsdCds, gakkaCds, ct);
        var bytes = await System.IO.File.ReadAllBytesAsync(tempFile, ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "研修会出席状況表.xlsx");
    }

    public async Task<IActionResult> Report_P002(CancellationToken ct = default)
    {
        var tempFile = _config["FilePaths:P002Temp"] ?? @"C:\Temp\研修会未受講者_temp.xlsx";
        var (startDate, endDate, fdsdCds, gakkaCds) = ReadConditions();
        await _reportService.GenerateP002Async(tempFile, startDate, endDate, fdsdCds, gakkaCds, ct);
        var bytes = await System.IO.File.ReadAllBytesAsync(tempFile, ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "研修会未受講者.xlsx");
    }

    public async Task<IActionResult> Report_P003(CancellationToken ct = default)
    {
        var templatePath = _config["FilePaths:P003Template"] ?? @"C:\Temp\研修実績一覧表_template.xlsx";
        var tempFile = _config["FilePaths:P003Temp"] ?? @"C:\Temp\研修実績一覧表_temp.xlsx";
        var (startDate, endDate, fdsdCds, gakkaCds) = ReadConditions();
        await _reportService.GenerateP003Async(templatePath, tempFile, startDate, endDate, fdsdCds, gakkaCds, ct);
        var bytes = await System.IO.File.ReadAllBytesAsync(tempFile, ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "研修実績一覧表.xlsx");
    }
}